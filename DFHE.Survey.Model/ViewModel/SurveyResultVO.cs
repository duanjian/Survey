using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFHE.Survey.Model
{
    public   class SurveyResultVO
    {
        public int SurveyId { get; set; }

        public string Username { get; set; }

        public string Mobile { get; set; }

        public int Age { get; set; }

        public int Gender { get; set; }

        public int Position { get; set; }

        public int Edu { get; set; }

        public string Refer { get; set; }

        public string Location { get; set; }

        public int Marital { get; set; }

        public string Suggestion { get; set; }

        public string Options { get; set; }
    }
}
