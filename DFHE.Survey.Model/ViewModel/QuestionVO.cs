using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFHE.Survey.Model
{
    public class QuestionVO
    {
        public int QuesionId { get; set; }

        public string QuestionTitle { get; set; }

        public bool? IsOrientation { get; set; }

        public List<OptionInfo> Options { get; set; }

        public int QuestionOptionsCount { get; set; }

        public int RequiredOptionsCount { get; set; }
    }
}
