using System.ComponentModel.DataAnnotations.Schema;

namespace TheConsultancyFirm.Models
{
    [NotMapped]
    public class ChartRecord
    {
        public ChartRecord(string date, int visits)
        {
            _date = date;
            _visits = visits;
        }
        private string _date;
        public string Date
        {
            get { return _date; }
            set { _date = value; }
        }
        private int _visits;
        public int Visits
        {
            get { return _visits; }
            set { _visits = value; }
        }
    }
}
