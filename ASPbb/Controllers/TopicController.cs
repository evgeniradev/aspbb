using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ASPbb.DataAccess.Repository.IRepository;
using ASPbb.Models;
using ASPbb.Utility;
using Microsoft.AspNetCore.Authorization;

namespace ASPbb.Controllers
{
    public class TopicController : Controller
    {
        private readonly ILogger<TopicController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public TopicController(
            ILogger<TopicController> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Show(int id)
        {
            Topic topic = _unitOfWork.Topic.Get(id);

            if (topic == null)
            {
                return NotFound();
            }

            IEnumerable<Post> posts = _unitOfWork
                .Post
                .GetAll(
                    p => p.TopicId == id,
                    orderBy: t => t.OrderBy(x => x.CreatedDate),
                    includeProperties: "Topic,ApplicationUser"
                );

            topic.Posts = (List<Post>)posts;

            return View(topic);
        }

        [Authorize]
        public IActionResult Upsert(int? id, int? forumId)
        {
            Topic topic = new Topic();

            if (id == null && forumId != null)
            {
                topic.ForumId = forumId.GetValueOrDefault();
            }

            if (id == null)
            {
                return View(topic);
            }

            if (!User.IsInRole(SD.Role_Admin))
            {
                return Forbid();
            }

            topic = _unitOfWork
                .Topic
                .GetFirstOrDefault(
                    t => t.Id == id, includeProperties: "Posts"
                );

            if (topic == null)
            {
                return NotFound();
            }

            return View(topic);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Upsert(Topic topic, string content)
        {
            if (ModelState.IsValid)
            {
                if (topic.Id == 0)
                {
                    topic.ApplicationUserId = SD.getCurrentUserId(User);
                    // Temporary fix. Content should never be empty. Validate.
                    if (content != null)
                    {
                        topic.PostsCount = 1;
                    }
                    _unitOfWork.Topic.Add(topic);
                    _unitOfWork.Save();

                    // Temporary fix. Content should never be empty. Validate.
                    if (content != null)
                    {
                        Post post = new Post
                        {
                            Content = content,
                            TopicId = topic.Id
                        };
                        post.ApplicationUserId = SD.getCurrentUserId(User);
                        _unitOfWork.Post.Add(post);
                    }
          
                    updateCountsOfForum(topic.ForumId);
                    _unitOfWork.Save();
                    TempData["Success"] = "Topic created successfully.";
                }
                else
                {
                    if (!User.IsInRole(SD.Role_Admin))
                    {
                        return Forbid();
                    }
                    _unitOfWork.Topic.Update(topic);
                    // Temporary fix. Content should never be empty. Validate.
                    if (content != null) { 
                        Post post = _unitOfWork
                            .Post
                            .GetFirstOrDefault(p => p.TopicId == topic.Id);

                        if (post == null)
                        {
                            post = new Post
                            {
                                Content = content,
                                TopicId = topic.Id
                            };
                            post.ApplicationUserId = SD.getCurrentUserId(User);
                            post.Content = content;
                            _unitOfWork.Post.Add(post);
                            _unitOfWork.Save();
                            topic.PostsCount = 1;
                            _unitOfWork.Topic.Update(topic);
                            updateCountsOfForum(topic.ForumId);
                        } else
                        {
                            post.Content = content;
                            _unitOfWork.Post.Update(post);
                        }
                    }
                    _unitOfWork.Save();
                    TempData["Success"] = "Topic updated successfully.";
                }
                
                return RedirectToAction("Show", "Forum", new { id = topic.ForumId });
            }
            return View(topic);
        }

        [HttpDelete]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Topic.Get(id);

            if (objFromDb == null)
            {
                TempData["Error"] = "Topic does not exist.";
                return Json(new { success = false });
            }

            _unitOfWork.Topic.Remove(objFromDb);
            _unitOfWork.Save();

            updateCountsOfForum(objFromDb.ForumId);
            _unitOfWork.Save();
            TempData["Success"] = "Topic deleted successfully.";

            return Json(new { success = true });
        }

        private void updateCountsOfForum(int forumId)
        {
            Forum forum = _unitOfWork
                .Forum
                .GetFirstOrDefault(
                    f => f.Id == forumId,
                    includeProperties: "Topics"
                );
            forum.TopicsCount = forum.Topics.Count();
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
