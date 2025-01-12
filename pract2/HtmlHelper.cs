using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace pract2
{
    public class HtmlHelper
    {
        // **רשימה של תגיות HTML תקניות**
        public List<string> HtmlTags { get; set; }

        // **רשימה של תגיות HTML תקניות שהן self-closing**
        public List<string> HtmlVoidTags { get; set; }

        // **אובייקט יחיד (Singleton) של המחלקה**
        // מוודא שקיים רק מופע אחד של HtmlHelper במהלך חיי התוכנית
        public readonly static HtmlHelper _instance = new HtmlHelper();

        // **גישה למופע היחיד**
        public static HtmlHelper Instance { get { return _instance; } }

        // **בנאי פרטי**
        // מונע יצירה של מופעים נוספים מחוץ למחלקה
        private HtmlHelper()
        {
            // **בונה את הנתיב עבור HtmlTags.json**
            string htmlTagsJsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JSON files", "HtmlTags.json");

            // בודק אם הקובץ קיים, ואם לא - זורק שגיאה
            if (!File.Exists(htmlTagsJsonPath))
            {
                throw new FileNotFoundException($"File not found: {htmlTagsJsonPath}");
            }

            // קורא את תוכן הקובץ וממיר אותו לרשימה של מחרוזות
            string htmlTagsJson = File.ReadAllText(htmlTagsJsonPath);
            HtmlTags = JsonSerializer.Deserialize<List<string>>(htmlTagsJson);

            // **בונה את הנתיב עבור HtmlVoidTags.json**
            string htmlVoidTagsJsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JSON files", "HtmlVoidTags.json");

            // בודק אם הקובץ קיים, ואם לא - זורק שגיאה
            if (!File.Exists(htmlVoidTagsJsonPath))
            {
                throw new FileNotFoundException($"File not found: {htmlVoidTagsJsonPath}");
            }

            // קורא את תוכן הקובץ וממיר אותו לרשימה של מחרוזות
            string htmlVoidTagsJson = File.ReadAllText(htmlVoidTagsJsonPath);
            HtmlVoidTags = JsonSerializer.Deserialize<List<string>>(htmlVoidTagsJson);
        }
    }
}
