using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace BaseClasses
{
    class Program
    {
        static readonly Warehouse Warehouse = new Warehouse();

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;     

            while (true)
                Command(Console.ReadLine());
        }

        static void Command(string command)         
        {
            Console.WriteLine(StringHandle(command));
            switch (StringHandle(command))
            {
                case "thoat":
                    Environment.Exit(0);
                    return;
                case "xoa man hinh":
                    Console.Clear();
                    return;
                default:
                    Console.WriteLine("Câu lệnh không hợp lệ!");
                    return;
            }
        }

        static string StringHandle(string s)
        {
            return Warehouse.SimplifyString(s);
        }

        static void AddItem(string s)
        {

        }
    }

    /// <summary>
    /// Kho chứa.
    /// </summary>
    public class Warehouse : List<Good>, IStringHandle
    {
        public delegate bool FilterDelegate(object obj);

        public new void Add(Good newGood)
        {
            foreach (Good good in this)
            {
                if (newGood.Compare(good, "Count"))
                {
                    good.Count += newGood.Count;
                    newGood = good;
                    return;
                }
            }

            newGood.StorageDate = DateTime.Now;
            newGood.Container = this;
            base.Add(newGood);
        }

        public IEnumerable<Good> Search(string idOrName)
        {
            foreach (Good good in this)
            {
                if (SimplifyString(good.Name).Contains(SimplifyString(idOrName)) || SimplifyString(good.Id).Contains(SimplifyString(idOrName)))
                {
                    yield return good;
                }
            }
        }

        public IEnumerable<Good> Search(string idOrName, FilterDelegate comparer)
        {
            foreach (Good good in Filter(comparer))
            {
                if (SimplifyString(good.Name).Contains(SimplifyString(idOrName)) || SimplifyString(good.Id).Contains(SimplifyString(idOrName)))
                {
                    yield return good;
                }
            }
        }

        public string SimplifyString(string str)
        {
            string normalizedText = str.Normalize(NormalizationForm.FormD);
            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            return Regex.Replace(regex.Replace(normalizedText, string.Empty).Normalize(NormalizationForm.FormC).ToLower(), @"\s+", " ").Trim();
        }

        public IEnumerable<Good> Filter(FilterDelegate comparer)
        {
            foreach (Good good in this)
            {
                if (comparer(good))
                {
                    yield return good;
                }
            }
        }

        public IEnumerable<Good> Filter<T>()
        {
            bool Compare(object obj)
            {
                if (typeof(T) == obj.GetType())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return Filter(Compare);
        }
    }

    /// <summary>
    /// Đồ thời trang.
    /// </summary>
    public class Good
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public DateTime StorageDate { get; set; }
        public Warehouse Container { get; set; }

        public bool Compare(Good good, params string[] ignore)
        {
            return ObjectComparable.Compare(this, good, ignore);
        }
    }

    /// <summary>
    /// Quần áo.
    /// </summary>
    public class Clothing : Good
    {
        public string Material { get; set; }
        public string Color { get; set; }
        public string Style { get; set; }
        public string Size { get; set; }
    }

    /// <summary>
    /// Giày dép.
    /// </summary>
    public class Footwear : Good
    {
        public string Material { get; set; }
        public string Color { get; set; }
        public string Style { get; set; }
        public string Size { get; set; }
    }

    /// <summary>
    /// Trang sức.
    /// </summary>
    public class Jewelry : Good
    {
        public string Material { get; set; }
        public string Style { get; set; }
        public float Weight { get; set; }
    }

    /// <summary>
    /// So sánh hai đối tượng dựạy vào đây để so sánh các thuộc tính của chúng.
    /// </summary>
    public static class ObjectComparable
    {
        public static bool Compare<T>(T obj1, T obj2, params string[] ignore)
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (!ignore.Contains(property.Name))
                {
                    object value1 = property.GetValue(obj1);
                    object value2 = property.GetValue(obj2);

                    if (!value1.Equals(value2))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

    /// <summary>
    /// Giao diện xử lý chuỗi.
    /// </summary>
    public interface IStringHandle
    {
        string SimplifyString(string str);
    }
}