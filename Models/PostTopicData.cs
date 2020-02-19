using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Site02.Models
{
    public class PostTopicData
    {
        public string Text { get; set; }
        public string Theme { get; set; }

        public bool CheckPostTheme(PostTopicData err)
        {
            if (Theme == "")
            {
                err.Theme = "Theme is required";
                return false;
            }
            else if (Theme.Length >= 100)
            {
                err.Theme = "Theme is too long";
                return false;
            }

            return true;
        }

        public bool CheckPostText(PostTopicData err)
        {
            if (Text == "")
            {
                err.Text = "Text is required";
                return false;
            }
            else if (Text.Length >= 1000)
            {
                err.Text = "Text is too long";
                return false;
            }

            return true;
        }

        public bool CheckPostData(PostTopicData err)
        {
            var checkTheme = CheckPostTheme(err);
            var checkText = CheckPostText(err);

            return checkTheme && checkText;
        }
    }
}
