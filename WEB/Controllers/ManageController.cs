using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;
using Web.Helpers;
using X.PagedList;
using AutoMapper;

namespace Web.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class ManageController : AbstractController
    {
        private readonly IDictionaryRepository _dictionaryRepository;

        private readonly IPlanRepository _planRepository;

        private readonly IFileRepository _fileRepository;

        private readonly ISettingRepository _settingRepository;

        private readonly IUserRepository _userRepository;

        private readonly IEmailService _emailService;

        public ManageController(ILoggerManager loggerManager,
                                IDictionaryRepository dictionaryRepository,
                                IPlanRepository planRepository,
                                IFileRepository fileRepository,
                                ISettingRepository settingRepository,
                                IUserRepository userRepository,
                                IEmailService emailService) : base(loggerManager)
        {
            _dictionaryRepository = dictionaryRepository;

            _planRepository = planRepository;

            _fileRepository = fileRepository;

            _settingRepository = settingRepository;

            _userRepository = userRepository;

            _emailService = emailService;
        }

        #region Dictionary

        [HttpGet]
        public IActionResult GetPositionList()
        {
            _loggerManager.Info($"GetPositionList is requested");

            var positions = _dictionaryRepository.GetPositions(false);

            ViewData["Title"] = "Positions";
            ViewData["HasPosition"] = true;

            return View("DictionaryList", positions);
        }

        [HttpGet]
        public IActionResult GetValueList()
        {
            _loggerManager.Info($"GetValueList is requested");

            var values = _dictionaryRepository.GetValues(false);

            ViewData["Title"] = "Values";
            ViewData["HasValue"] = true;

            return View("DictionaryList", values);
        }

        [HttpGet]
        public IActionResult GetStakeholderCategoryList()
        {
            _loggerManager.Info($"GetStakeholderCategoryList is requested");

            var stakeholderCategories = _dictionaryRepository.GetStakeholderCategories(false);

            ViewData["Title"] = "Stakeholder Categories";
            ViewData["HasStakeholderCategory"] = true;

            return View("DictionaryList", stakeholderCategories);
        }

        [HttpGet]
        public IActionResult GetStakeholderCriterionList()
        {
            _loggerManager.Info($"GetStakeholderCriterionList is requested");

            var stakeholderCriteria = _dictionaryRepository.GetStakeholderCriteria(false);

            ViewData["Title"] = "Stakeholder Criteria";
            ViewData["HasStakeholderCriteria"] = true;

            return View("DictionaryList", stakeholderCriteria);
        }

        [HttpPost]
        public IActionResult CreateDictionary(DictionaryDTO dictionary)
        {
            _loggerManager.Info($"CreateDictionary for {dictionary.Title} is requested");

            Response.StatusCode = StatusCodes.Status202Accepted;

            if (ModelState.IsValid)
            {
                var result = _dictionaryRepository.Create(dictionary, HttpContext.GetUserId());

                if (result)
                {
                    Response.StatusCode = StatusCodes.Status201Created;
                    _loggerManager.Info($"CreateDictionary for {dictionary.Title} successfully created a dictionary");
                }
                else
                {
                    _loggerManager.Warn($"CreateDictionary for {dictionary.Title} was unable to create a dictionary");
                }
            }
            else
            {
                _loggerManager.Warn($"CreateDictionary for {dictionary.Title} was invalid");
            }

            return PartialView("~/Views/Manage/Partials/_NewDictionary.cshtml");
        }

        [HttpPost]
        public IActionResult DeleteDictionary(int id)
        {
            _loggerManager.Info($"DeleteDictionary({id}) was requested");

            var result = false;

            result = _dictionaryRepository.Delete(id, HttpContext.GetUserId());

            if (result)
            {
                _loggerManager.Info($"DeleteDictionary({id}) successfully deleted");
            }
            else
            {
                _loggerManager.Warn($"DeleteDictionary({id}) was unable to delete");
            }

            return new JsonResult(new { result });
        }

        [HttpPost]
        public IActionResult ActivateDictionary(int id)
        {
            _loggerManager.Info($"ActivateDictionary({id}) was requested");

            var result = false;

            result = _dictionaryRepository.Activate(id, HttpContext.GetUserId());

            if (result)
            {
                _loggerManager.Info($"ActivateDictionary({id}) successfully activaed");
            }
            else
            {
                _loggerManager.Warn($"ActivateDictionary({id}) was unable to activate");
            }

            return new JsonResult(new { result });
        }

        [HttpPost]
        public IActionResult DisactivateDictionary(int id)
        {
            _loggerManager.Info($"DisactivateDictionary({id}) was requested");

            var result = false;

            result = _dictionaryRepository.Disactivate(id, HttpContext.GetUserId());

            if (result)
            {
                _loggerManager.Info($"DisactivateDictionary({id}) successfully disactivated");
            }
            else
            {
                _loggerManager.Warn($"DisactivateDictionary({id}) was unable to disactivate");
            }

            return new JsonResult(new { result });
        }

        #endregion

        #region Steps

        [HttpGet]
        public IActionResult GetIntroductionList()
        {
            _loggerManager.Info($"GetIntroductionList was requested");

            return RedirectToAction("GetIntroduction", new { stepIndex = Steps.Predeparture });
        }

        [HttpGet]
        public IActionResult GetIntroduction(string stepIndex)
        {
            _loggerManager.Info($"GetIntroduction({stepIndex}) was requested");

            if (!_planRepository.GetStepList().Contains(stepIndex))
            {
                _loggerManager.Warn($"GetIntroduction({stepIndex}) : Step index is wrong");
                return BadRequest();
            }

            var introduction = _planRepository.GetIntroduction(stepIndex);

            if (introduction == null)
            {
                introduction = new IntroductionDTO { Step = stepIndex };
            }

            return View("Introduction", introduction);
        }

        [HttpPost]
        public IActionResult UploadIntroduction(IntroductionDTO introduction)
        {
            _loggerManager.Info($"UploadIntroduction was requested");

            var files = HttpContext.Request.Form.Files;

            if (files == null || files.Count != 1)
            {
                ModelState.AddModelError(string.Empty, "A file is invalid");

                _loggerManager.Warn($"UploadIntroduction : A file is invalid");

                return View("Introduction", introduction);
            }

            //validates step's index
            if (!_planRepository.GetStepList().Contains(introduction.Step))
            {
                _loggerManager.Warn($"UploadIntroduction : Step index is wrong");
                return BadRequest();
            }

            var uploadlimit = int.Parse(_settingRepository.Get(Settings.FileUploadLimit));

            if (!ValidationHelper.ValidateFileSize(files[0], uploadlimit))
            {
                ModelState.AddModelError(string.Empty, "A file exceed a limit");

                _loggerManager.Warn($"UploadIntroduction : A file exceed a limit");

                return View("Introduction", introduction);
            }

            var userId = HttpContext.GetUserId();

            //upload a file in wwwroot directory
            var uploadRelPath = UploadHelper.Upload(files[0]);

            //creates a new file record
            var fileDto = _fileRepository.CreateNewFile(Path.GetFileNameWithoutExtension(files[0].FileName), Path.GetExtension(files[0].FileName), uploadRelPath, userId);

            //boolean that determines a result for updating of an introduction
            var result = false;

            
            if (fileDto != null)//A file record created, updates an introduction record
            {
                _loggerManager.Info($"UploadIntroduction : A file was created");

                //takes an older video version
                var oldVideo = _planRepository.GetIntroduction(introduction.Step)?.Video;

                //updaates an introduction for a step
                result = _planRepository.UpdateIntroduction(introduction.Step, fileDto.Id, userId);

                if (result && oldVideo != null)//If a update was succeessful and the introduction had a video before, deletes an older video
                {
                    //a result boolean
                    bool oldFileDelete;

                    //Deletes corresponding record in a repository and corresponding file in a directory, If any fails a result boolean is false
                    oldFileDelete = UploadHelper.Delete(oldVideo.Path);
                    oldFileDelete = _fileRepository.Delete(oldVideo.Id) && oldFileDelete;

                    if (oldFileDelete)
                    {
                        _loggerManager.Warn($"UploadIntroduction : An old file was deleted");
                    }
                    else
                    {
                        _loggerManager.Warn($"UploadIntroduction : An old file was not deleted");
                    }
                }
            }

            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong due to a server issue");
                _loggerManager.Warn($"UploadIntroduction was unable to update");

                return View("Introduction", introduction);
            }
            else
            {
                _loggerManager.Info($"UploadIntroduction successfully updated");
            }

            return RedirectToAction("GetIntroduction", new { stepIndex = introduction.Step });
        }

        [HttpGet]
        public IActionResult GetBlockList(string stepIndex)
        {
            _loggerManager.Info($"GetBlockList({stepIndex}) was requested");

            if (string.IsNullOrWhiteSpace(stepIndex) || !_planRepository.GetStepList().Contains(stepIndex))
            {
                ViewData["Step"] = Steps.Predeparture;
            }
            else
            {
                ViewData["Step"] = stepIndex;
            }

            return View("BlockEdit");
        }

        [HttpGet]
        public IActionResult BlockEdit(int id)
        {
            _loggerManager.Info($"BlockEdit({id}) was requested");

            var block = _planRepository.GetBlock(id);

            if (block == null)
            {
                _loggerManager.Warn($"BlockEdit({id}): A block was not found");
                return BadRequest();
            }

            ViewData["Step"] = block.Step;

            var blockEdit = new BlockEditDTO
            {
                Description = block.Description,
                Id = block.Id,
                Instruction = block.Instruction,
                Title = block.Title,
                Step = block.Step
            };

            _loggerManager.Info($"BlockEdit({id}): A block was successfully found");

            return View("BlockEdit", blockEdit);
        }

        [HttpPost]
        public IActionResult BlockEdit(BlockEditDTO blockEdit)
        {
            _loggerManager.Info($"BlockEdit() was requested");

            ViewData["Step"] = blockEdit.Step;

            if (ModelState.IsValid)
            {
                var result = _planRepository.UpdateBlock(blockEdit, HttpContext.GetUserId());

                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Something went wrong due to a server issue");

                    _loggerManager.Warn($"BlockEdit() was unable to update a block");
                }
                else
                {
                    ViewData["SuccessMessage"] = "Block successfully updated";

                    _loggerManager.Warn($"BlockEdit() successfully updated");
                }
            }
            else
            {
                _loggerManager.Warn($"BlockEdit() was invalid");
            }

            return View("BlockEdit", blockEdit);
        }

        #endregion

        #region Settings

        [HttpGet]
        public IActionResult GetSettingList()
        {
            _loggerManager.Info($"GetSettingList() was requested");

            return View("SettingList", _settingRepository.GetSettingList());
        }

        [HttpGet]
        public IActionResult EditSetting(string index)
        {
            _loggerManager.Info($"EditSetting({index}) GET was requested");

            var value = _settingRepository.Get(index);

            var settingDTO = new SettingDTO
            {
                Index = index,
                Value = value
            };

            return PartialView("~/Views/Manage/Partials/_EditSetting.cshtml", settingDTO);
        }

        [HttpPost]
        public IActionResult EditSetting(SettingDTO setting)
        {
            _loggerManager.Info($"EditSetting() POST was requested");

            var result = false;
            if (ModelState.IsValid)
            {
                result = _settingRepository.Set(setting.Index, setting.Value, HttpContext.GetUserId());

                if (result)
                {
                    _loggerManager.Info($"EditSetting() POST successfully updated");
                }
                else
                {
                    _loggerManager.Warn($"EditSetting() POST was unable to update");
                }
            }
            else
            {
                _loggerManager.Warn($"EditSetting() POST was invalid");
            }

            return PartialView("~/Views/Manage/Partials/_EditSetting.cshtml", setting);
        }

        #endregion

        #region User

        [HttpGet]
        public IActionResult GetUserList(int? page)
        {
            _loggerManager.Info($"GetUserList({page}) was requested");

            var parseResult = int.TryParse(_settingRepository.Get(Settings.PageSize), out int pageSize);

            if (!parseResult || pageSize <= 0)
            {
                pageSize = 5;
            }

            var skipCount = ((page ?? 1) - 1) * pageSize;
            var takeCount = pageSize;

            var planList = _userRepository.GetUserList(skipCount, takeCount, out int totalCount);

            var pagedList = new StaticPagedList<UserDTO>(planList, page ?? 1, pageSize, totalCount);

            _loggerManager.Info($"GetUserList({page}) returned a list");

            return View("UserList", pagedList);

        }

        [HttpPost]
        public IActionResult AddNewUser(NewUserDTO newUser)
        {
            if (ModelState.IsValid)
            {
                //check if an email of a new user is unique
                if (_userRepository.FindByCondition(u => u.Email == newUser.Email).Any())
                {
                    _loggerManager.Warn($"AddNewUser - an user with the email exists");

                    ModelState.AddModelError("Email", "An user with the email exists");

                    return PartialView("~/Views/Manage/Partials/_AddNewUser.cshtml");
                }

                newUser.Password = _userRepository.GeneratePassword();

                var user = _userRepository.AddNew(newUser, HttpContext.GetUserId());

                if (user == null)
                {
                    _loggerManager.Warn($"AddNewUser - Adding a new user failed");

                    ModelState.AddModelError(string.Empty, "Adding a new user failed");

                    return PartialView("~/Views/Manage/Partials/_AddNewUser.cshtml");
                }

                //send a password to a new user
                if (!_emailService.SendPasswordToUser(newUser.Password, user))
                {
                    _loggerManager.Error($"AddNewUser - Email was not sent to the user");

                    ModelState.AddModelError(string.Empty, "Email was not sent to the user");

                    return PartialView("~/Views/Manage/Partials/_AddNewUser.cshtml");
                }

                _loggerManager.Info($"AddNewUser succeesfully added a user");

                Response.StatusCode = StatusCodes.Status201Created;

            }
            else
            {
                _loggerManager.Warn($"AddNewUser request is invalid");
            }

            return PartialView("~/Views/Manage/Partials/_AddNewUser.cshtml");
        }

        [HttpGet]
        public IActionResult UserEdit(int id)
        {
            _loggerManager.Info($"UserEdit:Get was requested");

            if (id <= 0)
            {
                _loggerManager.Warn($"UserEdit:Get was bad request");

                return BadRequest();
            }

            var user = _userRepository.GetUserById(id);

            var userEdit = new UserEditDTO
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                PositionId = user.Position?.Id,
                Role = user.Role
            };

            return PartialView("~/Views/Manage/Partials/_UserEdit.cshtml", userEdit);
        }

        [HttpPost]
        public IActionResult UserEdit(UserEditDTO userEdit)
        {
            _loggerManager.Info($"UserEdit:Post was requested");

            Response.StatusCode = StatusCodes.Status202Accepted;

            if (ModelState.IsValid)
            {
                if (_userRepository.FindByCondition(u => u.Email == userEdit.Email && u.Id != userEdit.Id).Any())
                {
                    _loggerManager.Warn($"UserEdit - an user with the email exists");

                    ModelState.AddModelError("Email", "An user with the email exists");

                    return PartialView("~/Views/Manage/Partials/_UserEdit.cshtml");
                }

                var result = _userRepository.Update(userEdit, HttpContext.GetUserId());

                if (!result)
                {
                    _loggerManager.Warn($"UserEdit - updating an user failed");

                    ModelState.AddModelError(string.Empty, "Updating an user failed");

                    return PartialView("~/Views/Manage/Partials/_UserEdit.cshtml");
                }

                _loggerManager.Info($"UserEdit succeesfully updated a user");

                Response.StatusCode = StatusCodes.Status200OK;

            }
            else
            {
                _loggerManager.Warn($"UserEdit request is invalid");
            }

            return PartialView("~/Views/Manage/Partials/_UserEdit.cshtml");
        }

        [HttpPost]
        public IActionResult GeneratePassword(int id)
        {
            _loggerManager.Info($"GeneratePassword({id}) was requested");

            if (id <= 0)
            {
                _loggerManager.Warn($"GeneratePassword({id}) was bad request");

                return BadRequest();
            }

            var result = false;

            var user = _userRepository.GetUserById(id);

            if (user != null)
            {

                var password = _userRepository.GeneratePassword();

                result = _userRepository.ChangePassword(id, password, HttpContext.GetUserId());

                if (result)
                {
                    _loggerManager.Info($"GeneratePassword({id}) successfully updated password");
                }
                else
                {
                    _loggerManager.Warn($"GeneratePassword({id}) successfully updated password");
                }

                var emailResult = _emailService.SendPasswordToUser(password, user);

                if (emailResult)
                {
                    _loggerManager.Info($"GeneratePassword({id}) an email successfully sent");
                }
                else
                {
                    _loggerManager.Warn($"GeneratePassword({id}) an email was not sent");
                }

            }
            else
            {
                _loggerManager.Warn($"GeneratePassword({id}) no such user found");
            }

            return Json(new { result });
        }

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            _loggerManager.Info($"DeleteUser({id}) was requested");

            var result = _userRepository.Delete(id, HttpContext.GetUserId());

            if (result)
            {
                _loggerManager.Info($"DeleteUser({id}) successfully deleted");
            }
            else
            {
                _loggerManager.Warn($"DeleteUser({id}) was unable to delete");
            }

            return new JsonResult(new { result });
        }

        [HttpPost]
        public IActionResult ActivateUser(int id)
        {
            _loggerManager.Info($"ActivateUser({id}) was requested");

            var result = _userRepository.Activate(id, HttpContext.GetUserId());

            if (result)
            {
                _loggerManager.Info($"ActivateUser({id}) successfully activaed");
            }
            else
            {
                _loggerManager.Warn($"ActivateUser({id}) was unable to activate");
            }

            return new JsonResult(new { result });
        }

        [HttpPost]
        public IActionResult DisactivateUser(int id)
        {
            _loggerManager.Info($"DisactivateUser({id}) was requested");

            var result = _userRepository.Disactivate(id, HttpContext.GetUserId());

            if (result)
            {
                _loggerManager.Info($"DisactivateUser({id}) successfully disactivated");
            }
            else
            {
                _loggerManager.Warn($"DisactivateUser({id}) was unable to disactivate");
            }

            return new JsonResult(new { result });
        }

        #endregion

    }
}
