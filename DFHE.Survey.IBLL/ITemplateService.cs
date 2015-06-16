using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFHE.Survey.Model;

namespace DFHE.Survey.IBLL
{
    public interface ITemplateService
    {
        ResultVO GetTemplateList();

        ResultVO GetTemplateById(int id);

        ResultVO InsertTemplate(TemplateInfo template);

        ResultVO DeleteTemplateById(int id);

    }
}
