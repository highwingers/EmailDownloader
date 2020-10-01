using S22.Imap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityWebGroup.Services
{
    public class MailerSearch
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public string Sender { get; set; }

        public string Subject { get; set; }

        public bool SearchByDateRange { get; set; }
        public bool Unread { get; set; }

        /// <summary>
        ///      SentSince = Date  = DateFrom
        ///      SentBefore = Date + 1 = DateTO
        /// </summary>
        /// <returns></returns>
        public SearchCondition SetSearchConditions()
        {

            SearchCondition result = SearchCondition.All();

            if (this.SearchByDateRange)
            {
                result = result.And(SearchCondition.SentBefore(  new DateTime(DateTo.Year, DateTo.Month, DateTo.Day ).AddDays(1))
                               .And(SearchCondition.SentSince(new DateTime(DateFrom.Year, DateFrom.Month, DateFrom.Day))));

            }
            if (this.Unread)
            {
                result = result.And(SearchCondition.Unseen());
            }
            if (!string.IsNullOrEmpty(this.Sender))
            {
                result = result.And(SearchCondition.From(this.Sender));
            }
            if (!string.IsNullOrEmpty(this.Subject))
            {
                result = result.And(SearchCondition.Subject(this.Subject));
            }



            return result;
        }
    }
}
