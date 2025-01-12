using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pract2
{
    public class HtmlElement
    {
        // **מאפיינים של אלמנט HTML**
        // שם התגית (לדוגמה: "div", "span", וכו')
        public string Name { get; set; }

        // מזהה ייחודי של האלמנט (id)
        public string Id { get; set; }

        // רשימת אטריביוטים (attributes) של האלמנט
        public List<string> Attributes { get; set; }

        // רשימת המחלקות (classes) שמוגדרות על האלמנט
        public List<string> Classes { get; set; }

        // תוכן פנימי של האלמנט (כל מה שנמצא בין התגיות הפותחת והסוגרת)
        public string InnerHtml { get; set; }

        // הפניה לאלמנט האב של האלמנט הנוכחי
        public HtmlElement Parent { get; set; }

        // רשימת אלמנטים שהם ילדים של האלמנט הנוכחי
        public List<HtmlElement> Children { get; set; }

        // **אתחול ברירת מחדל של המאפיינים**
        // כשיוצרים אלמנט חדש, המאפיינים מאותחלים לערכים ריקים
        public HtmlElement()
        {
            Attributes = new List<string>();
            Classes = new List<string>();
            Parent = null;
            Children = new List<HtmlElement>();
        }

        // **מתודה לקבלת כל הצאצאים של האלמנט**
        // השיטה משתמשת בתור (queue) כדי לעבור על כל עץ האלמנטים.
        // כל אלמנט שמתווסף לרשימת הצאצאים יוחזר (yield).
        public IEnumerable<HtmlElement> Descendants()
        {
            // יוצרים תור ומכניסים את האלמנט הנוכחי (this)
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);

            // מבצעים לולאה כל עוד יש אלמנטים בתור
            while (queue.Count > 0)
            {
                // מוציאים את האלמנט הראשון בתור
                HtmlElement currentElement = queue.Dequeue();

                // מוסיפים את כל הילדים של האלמנט הנוכחי לתור
                foreach (var child in currentElement.Children)
                {
                    queue.Enqueue(child);
                }

                // מחזירים את האלמנט הנוכחי
                yield return currentElement;
            }
        }

        // **מתודה לקבלת כל האבות של האלמנט**
        // פונקציה שמחזירה את כל שרשרת האבות של האלמנט.
        public IEnumerable<HtmlElement> Ancestors()
        {
            // מתחילים מהאלמנט הנוכחי
            HtmlElement child = this;

            // מבצעים לולאה כל עוד יש לאלמנט אב (Parent)
            while (child.Parent != null)
            {
                // מחזירים את האב הנוכחי
                yield return child.Parent;

                // ממשיכים לעלות לעבר האב הבא
                child = child.Parent;
            }
        }
    }
}
