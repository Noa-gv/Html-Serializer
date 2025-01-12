using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace pract2
{
    public class Selector
    {
        // **מאפיינים של selector**
        public string TagName { get; set; } // שם התגית (לדוגמה: div, span)
        public string Id { get; set; } // מזהה (Id) של האלמנט
        public List<string> Classes { get; set; } // רשימת המחלקות (Classes) של האלמנט
        public Selector Parent { get; set; } // Selector האב
        public Selector Child { get; set; } // Selector הבן

        // **אתחול ברירת מחדל**
        public Selector()
        {
            TagName = null;
            Id = null;
            Classes = new List<string>();
            Child = null;
            Parent = null;
        }

        // **פונקציה לבניית עץ selector מתוך מחרוזת**
        public static Selector selectorElement(string select)
        {
            // פיצול המחרוזת לרמות באמצעות רווחים
            List<string> levels = select.Split(' ').ToList();

            // יצירת ה-Selector הראשי
            Selector rootSelector = new Selector();
            Selector currentSelector = rootSelector;

            // טעינת רשימת התגיות מ- HtmlHelper
            List<string> allTags = HtmlHelper.Instance.HtmlTags;

            foreach (var level in levels)
            {
                // פיצול הרמה הנוכחית לפילטרים (תגיות, מזהים ומחלקות)
                string[] filters = Regex.Split(level, @"(?=[#.])")
                                        .Where(s => s.Length > 0).ToArray();

                foreach (var filter in filters)
                {
                    // אם הפילטר הוא מזהה
                    if (filter.StartsWith("#"))
                    {
                        currentSelector.Id = filter.Substring(1); // מסירים את הסימן #
                    }
                    // אם הפילטר הוא מחלקה
                    else if (filter.StartsWith("."))
                    {
                        currentSelector.Classes.Add(filter.Substring(1)); // מסירים את הסימן .
                    }
                    // אם הפילטר הוא שם תגית חוקית
                    else if (allTags.Contains(filter))
                    {
                        currentSelector.TagName = filter;
                    }
                    else
                    {
                        // אם הפילטר אינו חוקי
                        Console.WriteLine("Error: Invalid filter");
                    }

                    // יצירת Selector חדש וקישורו ל-Selector הנוכחי
                    Selector newSelector = new Selector
                    {
                        Parent = currentSelector
                    };
                    currentSelector.Child = newSelector;
                    currentSelector = newSelector;
                }
            }
            return rootSelector;
        }

        // **השוואה בין selector לבין HtmlElement**
        public override bool Equals(object obj)
        {
            if (obj is HtmlElement element)
            {
                // בדיקת התאמת Id ו-TagName (אם הם קיימים ב-Selector)
                if ((this.Id == null || this.Id.Equals(element.Id)) &&
                    (this.TagName == null || this.TagName.Equals(element.Name)))
                {
                    // אם אין מחלקות ב-Selector, האלמנט מתאים
                    if (this.Classes.Count == 0)
                        return true;

                    // בדיקה שכל המחלקות ב-Selector קיימות באלמנט
                    foreach (var c in this.Classes)
                    {
                        if (!element.Classes.Contains(c))
                        {
                            return false;
                        }
                    }
                    return true; // כל המחלקות מתאימות
                }
                return false; // Id או TagName לא התאימו
            }
            return false; // האובייקט אינו HtmlElement
        }
    }
}

