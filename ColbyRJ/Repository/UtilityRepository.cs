namespace ColbyRJ.Repository
{
    public class UtilityRepository : IUtilityRepository
    {
        public UtilityRepository()
        {

        }

        public async Task<string> GetCategoryTopic(string category, string topic)
        {
            var categoryTopic = category;
            if (topic.Length > 0)
            {
                categoryTopic = category + " - " + topic;
            }

            return categoryTopic;
        }

        public async Task<string> GetGroupedBy(string category, string section, string topic)
        {
            var groupedBy = category;
            if (section.Length > 0)
            {
                groupedBy = category + " - " + section;

                if (topic.Length > 0)
                {
                    groupedBy = category + " - " + section + " - " + topic;
                }
            }

            return groupedBy;
        }

        public async Task<string> GetYearMon(string yearStr, string monStr)
        {
            string yearMon = yearStr;

            try
            {
                int monInt = int.Parse(monStr);
                if (monInt > 0)
                {
                    yearMon = yearStr + "-" + monStr;
                }
            }
            catch { }

            return yearMon;
        }

        public async Task<string> GetYearMonth(string yearStr, string monStr)
        {
            string yearMonth = yearStr;

            try
            {
                int monInt = int.Parse(monStr);
                if (monInt > 0)
                {
                    if (monStr == "01")
                    {
                        yearMonth = yearStr + " - Jan";
                    }
                    else if (monStr == "02")
                    {
                        yearMonth = yearStr + " - Feb";
                    }
                    else if (monStr == "03")
                    {
                        yearMonth = yearStr + " - Mar";
                    }
                    else if (monStr == "04")
                    {
                        yearMonth = yearStr + " - Apr";
                    }
                    else if (monStr == "05")
                    {
                        yearMonth = yearStr + " - May";
                    }
                    else if (monStr == "06")
                    {
                        yearMonth = yearStr + " - Jun";
                    }
                    else if (monStr == "07")
                    {
                        yearMonth = yearStr + " - Jul";
                    }
                    else if (monStr == "08")
                    {
                        yearMonth = yearStr + " - Aug";
                    }
                    else if (monStr == "09")
                    {
                        yearMonth = yearStr + " - Sep";
                    }
                    else if (monStr == "10")
                    {
                        yearMonth = yearStr + " - Oct";
                    }
                    else if (monStr == "11")
                    {
                        yearMonth = yearStr + " - Nov";
                    }
                    else
                    {
                        yearMonth = yearStr + " - Dec";
                    }
                }
            }
            catch { }

            return yearMonth;
        }
    }
}
