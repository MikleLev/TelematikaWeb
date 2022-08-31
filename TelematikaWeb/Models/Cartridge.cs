using System.ComponentModel.DataAnnotations;

namespace TelematikaWEB.Models
{
    public class Cartridge
    {
        public int id { set; get; }

        [Range(1, 8)]
        public int number { set; get; }

        [RegularExpression("5000|2000|1000|500|200|100", ErrorMessage = "Некорректная купюра")]
        public int value { set; get; }

        public int count { set; get; }

        public bool isAvailable { set; get; }
    }
}