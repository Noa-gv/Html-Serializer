using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;
using pract2;

// טוען HTML מ-URL מסוים
var html = await Load("https://malkabruk.co.il/learn");

// סיריאליזציה של HTML לעץ אלמנטים
static HtmlElement HtmiSerializer(string html)
{
    // ניקוי HTML - החלפת רווחים מרובים ברווח אחד
    var cleanHtml = new Regex("\\s").Replace(html, " ");

    // פיצול ה-HTML לרשימה לפי תגיות
    var htmlLines = new Regex("<(.*?)>").Split(cleanHtml).Where(s => s.Length > 0);
    htmlLines = htmlLines.Where(line => !string.IsNullOrWhiteSpace(line)).ToList();

    // טעינת כל התגיות והתגיות העצמאיות מקובצי JSON באמצעות HtmlHelper
    List<string> allTags = HtmlHelper.Instance.HtmlTags;
    List<string> voidTags = HtmlHelper.Instance.HtmlVoidTags;

    // יצירת האלמנט הראשי של עץ ה-HTML
    HtmlElement root = new HtmlElement() { };
    HtmlElement currentElement = root;

    // עיבוד כל שורה בקוד ה-HTML
    foreach (var line in htmlLines)
    {
        string firstWord = line.Split(" ")[0]; // מילת הפתיחה של השורה

        // טיפול בתגית הפתיחה הראשית (html)
        if (firstWord == "html")
        {
            currentElement.Name = firstWord;

            // חילוץ אטריביוטים של התגית
            var myAttributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(line);
            foreach (Match attribute in myAttributes)
            {
                string attributeName = attribute.Groups[1].Value;
                string attributeValue = attribute.Groups[2].Value;

                // שמירת אטריביוטים לפי הסוג
                if (attributeName == "class")
                {
                    currentElement.Classes = attributeValue.Split(' ').ToList();
                }
                else if (attributeName == "id")
                {
                    currentElement.Id = attributeValue;
                }
                else
                {
                    currentElement.Attributes.Add(attribute.Name + " = " + attribute.Value);
                }
            }
        }
        // טיפול בתגית הסגירה של המסמך (html/)
        else if (firstWord == "html/")
        {
            Console.WriteLine("we finish ");
        }
        // טיפול בתגית סגירה רגילה
        else if (firstWord.StartsWith("/"))
        {
            if (currentElement.Parent != null)
            {
                currentElement = currentElement.Parent;
            }
        }
        // טיפול בתגיות פתיחה אחרות
        else if (allTags.Contains(firstWord))
        {
            HtmlElement newElement = new HtmlElement();
            newElement.Parent = currentElement;
            currentElement.Children.Add(newElement);
            newElement.Name = firstWord;

            // חילוץ אטריביוטים של התגית
            var myAttributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(line);
            foreach (Match attribute in myAttributes)
            {
                string attributeName = attribute.Groups[1].Value;
                string attributeValue = attribute.Groups[2].Value;

                if (attributeName == "class")
                {
                    newElement.Classes = attributeValue.Split(' ').ToList();
                }
                else if (attributeName == "id")
                {
                    newElement.Id = attributeValue;
                }
                else
                {
                    newElement.Attributes.Add(attribute.Name + " = " + attribute.Value);
                }
            }

            // אם זה לא תגית עצמאית ולא מסתיימת ב-"/", מעבירים אליה את השליטה
            if (!(line.EndsWith("/") || voidTags.Contains(firstWord)))
            {
                currentElement = newElement;
            }
        }
        // במידה ואין תגית - מתייחסים לתוכן כ-InnerHtml
        else
        {
            currentElement.InnerHtml = line;
        }
    }

    return root; // מחזירים את עץ ה-HTML
}

// בונים עץ HTML מהתוכן הטעון
HtmlElement htmlTree = HtmiSerializer(html);

// פונקציה לטעינת HTML מ-URL
async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}

// חיפוש אלמנטים בעזרת Selector
Selector sel = new Selector();
string search = "div .down-content"; // דוגמה לחיפוש אלמנטים עם המחלקה "down-content" בתוך div

// יצירת Selector מתאים לחיפוש
Selector rooti = Selector.selectorElement(search);

// חיפוש האלמנטים התואמים בעץ ה-HTML
List<HtmlElement> result = htmlTree.func1(rooti);
Console.WriteLine("number = " + result.Count); // הדפסת מספר התוצאות שנמצאו

// כאן מוסיפים את ההדפסה של התגיות שנמצאו
foreach (var el in result)
{
    Console.WriteLine($"Element: <{el.Name}> | Id: {el.Id ?? "none"} | Classes: {(el.Classes.Count > 0 ? string.Join(", ", el.Classes) : "none")} | InnerHtml: {el.InnerHtml ?? "none"}");
}

// שיטות הרחבה לחיפוש רכיבי HTML
public static class exmentionMetod
{
    // חיפוש אלמנטים תואמים ל-Selector
    public static List<HtmlElement> func1(this HtmlElement element, Selector selector)
    {
        HashSet<HtmlElement> matcheSet = new HashSet<HtmlElement>();
        func2(element, selector, matcheSet);
        return matcheSet.ToList();
    }

    // פונקציה רקורסיבית למציאת אלמנטים תואמים
    public static void func2(HtmlElement currentElement, Selector selector, HashSet<HtmlElement> set)
    {
        // חיפוש צאצאים של האלמנט הנוכחי
        IEnumerable<HtmlElement> children = currentElement.Descendants();
        List<HtmlElement> listOfMatches = new List<HtmlElement>();

        foreach (var child in children)
        {
            // בדיקה אם האלמנט מתאים ל-Selector
            if (selector.Equals(child))
            {
                listOfMatches.Add(child);
            }
        }

        // אם אין המשך לשרשרת ה-Selector, מוסיפים את התוצאות
        if (selector.Child.Child == null)
        {
            set.UnionWith(listOfMatches);
            return;
        }
        else
        {
            // ממשיכים לחפש בצאצאים של התוצאות הנוכחיות
            foreach (var match in listOfMatches)
            {
                func2(match, selector.Child, set);
            }
        }
    }
}
