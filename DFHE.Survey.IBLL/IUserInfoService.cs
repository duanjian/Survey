
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFHE.Survey.IBLL;
using DFHE.Survey.Model;

namespace DFHE.Survey.IBLL
{
	public interface IUserInfoService
	{
	    ResultVO Login(UserInfo user);

	    ResultVO GetUserList();

	    ResultVO GetUserInfoById(int id);

	    ResultVO InsertUserInfo(UserInfo user);

	    ResultVO DeleteUserInfo(int id);

	}
}
