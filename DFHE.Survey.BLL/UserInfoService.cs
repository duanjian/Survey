
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DFHE.Survey.DAL;
using DFHE.Survey.IDAL;
using DFHE.Survey.IBLL;
using DFHE.Survey.Model;
using DFHE.Survey.Utility;

namespace DFHE.Survey.BLL
{
    public class UserInfoService : IUserInfoService
    {
        private IUserInfoRepository _userInfoRepository;
        private IUnitOfWork _unitOfWork;

        public UserInfoService(IUserInfoRepository userInfoRepository, IUnitOfWork unitOfWork)
        {
            _userInfoRepository = userInfoRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ResultVO Login(UserInfo user)
        {
            ResultVO result = new ResultVO() { Result = 0 };

            try
            {
                var tmpPwd = EncryptHelper.DecodeBase64(EncryptHelper.DecodeBase64(user.Password)).Md5();

                var userInfo =
                    _userInfoRepository.LoadEntities(u => u.UserName == user.UserName && u.Password == tmpPwd && u.Deleted == false)
                        .FirstOrDefault();

                var tmptt = EncryptHelper.EncodeBase64(string.Format("{0}:{1}", userInfo.UserName, userInfo.Password));

                if (userInfo != null)
                {
                    result.Data = new { tt = tmptt, UserInfo = new { userInfo.UserName, userInfo.RealName }};
                    result.Result = 1;
                }
            }
            catch (Exception ex)
            {
                result.ErrorMsg = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        public ResultVO GetUserList()
        {
            ResultVO result = new ResultVO() { Result = 0 };

            try
            {
                var userlist = _userInfoRepository.LoadEntities(u => u.Deleted == false).Select(u => new { u.UserId, u.UserName, u.RealName, u.CreateTime }).ToList();
                result.Data = userlist;
                result.Result = 1;
            }
            catch (Exception ex)
            {
                result.ErrorMsg = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 根据ID得到用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResultVO GetUserInfoById(int id)
        {
            ResultVO result = new ResultVO() { Result = 0 };

            try
            {
                var userInfo =
                    _userInfoRepository.LoadEntities(u => u.UserId == id && u.Deleted == false).FirstOrDefault();

                result.Data = userInfo;
                result.Result = 1;

            }
            catch (Exception ex)
            {
                result.ErrorMsg = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 插入用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ResultVO InsertUserInfo(UserInfo user)
        {
            ResultVO result = new ResultVO() { Result = 0 };

            try
            {
                user.Password = user.Password.Md5();
                user.CreateTime = DateTime.Now;
                user.Deleted = false;

                var existedUser =
                    _userInfoRepository.LoadEntities(u => u.UserName == user.UserName && u.Deleted == false)
                        .FirstOrDefault();
                _unitOfWork.Commit();

                if (existedUser != null)
                {
                    result.Result = 2;
                    return result;
                }

                var insertedUser = _userInfoRepository.Insert(user);
                _unitOfWork.Commit();
                result.Data = new { insertedUser.UserId, insertedUser.UserName, insertedUser.RealName, insertedUser.CreateTime };
                result.Result = 1;
            }
            catch (Exception ex)
            {
                result.ErrorMsg = ex.Message;
            }

            return result;
        }

        ///// <summary>
        ///// 修改用户信息
        ///// </summary>
        ///// <param name="user"></param>
        ///// <returns></returns>
        //public ResultVO UpdateUserInfo(UserInfo user)
        //{
        //    ResultVO result = new ResultVO() { Result = 0 };

        //    try
        //    {
        //        var tmpUser = _userInfoRepository.LoadEntities(u => u.UserId == user.UserId).FirstOrDefault();
        //        tmpUser.Password = user.Password;
        //        _userInfoRepository.Update(tmpUser);
        //        result.Result = 1;

        //    }
        //    catch (Exception ex)
        //    {
        //        result.ErrorMsg = ex.Message;
        //    }

        //    return result;
        //}

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ResultVO DeleteUserInfo(int id)
        {
            ResultVO result = new ResultVO() { Result = 0 };

            try
            {               
                var tmpUser = _userInfoRepository.LoadEntities(u => u.UserId == id).FirstOrDefault();

                if (tmpUser.UserId == 1 && tmpUser.UserName == "admin")
                {
                    result.Result = 2;
                    result.Data = "系统管理员帐号，无法删除";
                    return result;
                }

                tmpUser.Deleted = true;
                _userInfoRepository.Update(tmpUser);
                _unitOfWork.Commit();
                result.Result = 1;
            }
            catch (Exception ex)
            {
                result.ErrorMsg = ex.Message;
            }

            return result;
        }
    }
}
