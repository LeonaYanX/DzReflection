using System.Text;
using System.Reflection;

namespace DzReflection
{
    [AttributeUsage(AttributeTargets.Field)]
    class CustomNameAttribute : Attribute 
    {
        public string name = "CustomName";
        public CustomNameAttribute(string name)
        {
            this.name = name;
        }
        public CustomNameAttribute() { }
    }
    [AttributeUsage(AttributeTargets.Field)]
    class CustomValueFieldAttribute : Attribute 
    {
        string value = String.Empty;
        public CustomValueFieldAttribute(string value) { this.value = value; }

    }


    class TestClassForAttribute 
    {
        [CustomName()]
        int Id = default(int);
       
        [CustomValueField("SomeText")]
        string Text = String.Empty;

    }
    internal class Program
    {
        static string ObjectToString(Object obj) 
        {
            StringBuilder sb = new StringBuilder();
           Type objType= obj.GetType();
            sb.Append(objType.Name);
            var objFiels = objType.GetFields();
            
            
            foreach ( var fiel in objFiels ) 
            {
                if (fiel.GetCustomAttribute<CustomNameAttribute>()==null)
                {
                    sb.Append("|");
                   sb.Append(fiel.Name);
                    sb.Append("|");
                    sb.Append(fiel.GetValue(obj));
                }
                sb.Append("|");
              sb.Append(new CustomNameAttribute().name);
                sb.Append("|");
                sb.Append(fiel.GetValue(obj));
            }
            return sb.ToString();
        }

       static Object StringToObject(string str) 
        {
           str = str.Trim();
            var stringArr = str.Split("|");
            var obj = Activator.CreateInstance(null, stringArr[0]).Unwrap;
            var fields =obj.GetType().GetFields();
            foreach( var field in fields ) 
            {
                if (field.GetCustomAttribute<CustomValueFieldAttribute>() == null) 
                {
                    field.SetValue(obj ,default);
                }
                field.SetValue(obj ,new CustomValueFieldAttribute("SomeValue"));
            }
            return obj;
        }

        static void Main(string[] args)
        {
            TestClassForAttribute testClass = new TestClassForAttribute();
            string objString =ObjectToString(testClass);
            Console.WriteLine(objString);
            var obj =StringToObject(objString) as TestClassForAttribute;
            Console.WriteLine(obj);
            


        }
    }
}
