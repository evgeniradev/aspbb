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
    public class ForumController : Controller
    {
        private readonly ILogger<ForumController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public ForumController(
            ILogger<ForumController> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Forum> forumList = _unitOfWork
                .Forum
                .GetAll(
                    orderBy: t => t.OrderBy(x => x.CreatedDate),
                    includeProperties: "Topics"
                 );

            foreach(Forum forum in forumList)
            {
                forum.Topics = (List<Topic>)_unitOfWork
                    .Topic
                    .GetAll(
                    t => t.ForumId == forum.Id,
                    limit: 1,
                    orderBy: t => t.OrderByDescending(x => x.UpdatedDate)
                );


                if (forum.Topics.Count() > 0)
                {
                    Topic latestTopic = forum.Topics.First();

                    latestTopic.Posts = (List<Post>)_unitOfWork
                        .Post
                        .GetAll(
                            t => t.TopicId == latestTopic.Id,
                            limit: 1,
                            orderBy: t => t.OrderByDescending(x => x.UpdatedDate),
                            includeProperties: "ApplicationUser"
                        );
                }
            }

            return View(forumList);
        }

        public IActionResult Show(int id)
        {
            Forum forum = _unitOfWork.Forum.Get(id);

            if (forum == null)
            {
                return NotFound();
            }

            IEnumerable<Topic> topics = _unitOfWork
                .Topic
                .GetAll(
                    t => t.ForumId == id,
                    orderBy: t => t.OrderByDescending(x => x.UpdatedDate),
                    includeProperties: "Forum"
                );

            foreach (Topic topic in topics)
            {
                topic.Posts = (List<Post>)_unitOfWork
                    .Post
                    .GetAll(
                        t => t.TopicId == topic.Id,
                        limit: 1,
                        orderBy: t => t.OrderByDescending(x => x.UpdatedDate),
                        includeProperties: "ApplicationUser"
                    );
            }

            forum.Topics = (List<Topic>)topics;

            return View(forum);
        }

        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Upsert(int? id)
        {
            Forum forum = new Forum();

            if (id == null)
            {
                return View(forum);
            }

            forum = _unitOfWork.Forum.Get(id.GetValueOrDefault());

            if (forum == null)
            {
                return NotFound();
            }

            return View(forum);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Upsert(Forum forum)
        {
            if (ModelState.IsValid)
            {
                if (forum.Id == 0)
                {
                    forum.ApplicationUserId = SD.getCurrentUserId(User);
                    _unitOfWork.Forum.Add(forum);
                    TempData["Success"] = "Forum created successfully.";
                }
                else
                {
                    _unitOfWork.Forum.Update(forum);
                    TempData["Success"] = "Forum updated successfully.";
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(forum);
        }

        [HttpDelete]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Forum.Get(id);

            if (objFromDb == null)
            {
                TempData["Error"] = "Forum does not exist.";
                return Json(new { success = false });
            }

            _unitOfWork.Forum.Remove(objFromDb);
            _unitOfWork.Save();
            TempData["Success"] = "Forum deleted successfully.";

            return Json(new { success = true });
        }
    }
}
