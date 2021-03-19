using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ASPbb.DataAccess.Repository.IRepository;
using ASPbb.Models;
using ASPbb.Utility;
using Microsoft.AspNetCore.Authorization;

namespace ASPbb.Controllers
{
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public PostController(
            ILogger<PostController> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        public IActionResult Upsert(int? id, int? topicId)
        {
            Post post = new Post();

            if (id == null && topicId != null)
            {
                post.TopicId = topicId.GetValueOrDefault();
            }

            if (id == null)
            {
                return View(post);
            }

            if (!User.IsInRole(SD.Role_Admin))
            {
                return Forbid();
            }

            post = _unitOfWork.Post.GetFirstOrDefault(t => t.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Upsert(Post post, string content)
        {
            if (ModelState.IsValid)
            {
                if (post.Id == 0)
                {
                    post.ApplicationUserId = SD.getCurrentUserId(User);
                    _unitOfWork.Post.Add(post);
                    updateCountsOfForumAndTopic(post.TopicId);
                    TempData["Success"] = "Post created successfully.";
                }
                else
                {
                    if (!User.IsInRole(SD.Role_Admin))
                    {
                        return Forbid();
                    }

                    _unitOfWork.Post.Update(post);
                    TempData["Success"] = "Post updated successfully.";
                }
                _unitOfWork.Save();
                return RedirectToAction("Show", "Topic", new { id = post.TopicId });
            }
            return View(post);
        }

        [HttpDelete]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Delete(int id)
        {
            var postFromDb = _unitOfWork.Post.Get(id);

            if (postFromDb == null)
            {
                TempData["Error"] = "Post does not exist.";
                return Json(new { success = false });
            }

            _unitOfWork.Post.Remove(postFromDb);
            _unitOfWork.Save();

            updateCountsOfForumAndTopic(postFromDb.TopicId);

            _unitOfWork.Save();
            TempData["Success"] += "Post deleted successfully.";

            return Json(new { success = true });
       
        }

        private void updateCountsOfForumAndTopic(int topicId)
        {
            Topic topic = _unitOfWork
                .Topic
                .GetFirstOrDefault(
                    t => t.Id == topicId,
                    includeProperties: "Posts"
                );
            topic.PostsCount = topic.Posts.Count();
            _unitOfWork.Topic.Update(topic);

            Forum forum = _unitOfWork
                .Forum
                .GetFirstOrDefault(
                    f => f.Id == topic.ForumId,
                    includeProperties: "Topics"
                );
            int PostsCount = 0;
            foreach (Topic topicItem in forum.Topics)
            {
                int count = _unitOfWork
                    .Topic
                    .GetFirstOrDefault(
                        t => t.Id == topicItem.Id,
                        includeProperties: "Posts"
                    )
                    .Posts
                    .Count();
                PostsCount += count;
            }
            forum.PostsCount = PostsCount;
            _unitOfWork.Forum.Update(forum);
        }
    }
}
