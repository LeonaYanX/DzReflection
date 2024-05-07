using System.Text;
using System.Reflection;

namespace DzReflection
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    class CustomNameAttribute : Attribute 
    {
        public string Name { get; }
        public CustomNameAttribute(string name)
        {
          Name = name;
        }
       
    }

    class TestClassForAttribute
    {
        [CustomName("CustomName")]
        public int I = 0;
    }
        public static class CustomSerializer 
        {
            public static string ObjectToString(Object obj) 
            {
               StringBuilder sb = new StringBuilder();
                Type type = obj.GetType();

                // Getting all fields

                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                   string name = field.Name;
                    var attr = new CustomNameAttribute("CustomName");
                    //var attr = field.GetCustomAttributes<CustomNameAttribute>();
                    if (attr != null) 
                    {
                        name = attr.Name;
                    }
                    sb.Append($"{name}:{field.GetValue(obj)}");
                }


                 // getting all properties


            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                string name = prop.Name;
                var attr = new CustomNameAttribute("CustomName");
               // var attr = prop.GetCustomAttributes<CustomNameAttribute>();
                if(attr != null) 
                {
                    name = attr.Name;
                }

                sb.Append($"{name}:{prop.GetValue(obj)}");
            }
              return sb.ToString().Trim();
            }

        public static T StringToObject<T>(string str) where T : new()
        {
             T obj = new T();
            Type type = typeof(T);
            string[] keyValuePairs = str.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var keyValuePair in keyValuePairs )
            {
                string[] parts = keyValuePair.Split(':');
                if (parts.Length != 2)
                {
                    continue; // the string is not correct
                }
                string key = parts[0];
                string value = parts[1];

                FieldInfo ?field = type.GetField(key);
                PropertyInfo ?prop = type.GetProperty(key);

                if (prop == null && field==null) 
                {
                    // Searching with attribute CustomName

                    foreach (var f in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                    {
                        var att = new CustomNameAttribute("CustomName");
                       // var att = f.GetCustomAttributes<CustomNameAttribute>();
                        if (att != null && att.Name==key)
                        {
                            field = f;
                            break;
                        }
                    }

                    foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)) 
                    {
                        var attr = new CustomNameAttribute("CustomName");
                      // var attr = p.GetCustomAttributes<CustomNameAttribute>();
                        if (attr != null && attr.Name == key) 
                        {
                          prop = p; 
                            break;    
                        }
                    }
                }

                if (field != null) 
                {
                    object convertedValue = Convert.ChangeType(value,field.FieldType);
                    field.SetValue(type,convertedValue);
                }
                else if(prop != null)
                {
                    object convertedValue = Convert.ChangeType(value, prop.PropertyType);
                    prop.SetValue(type,convertedValue);
                }
            }

            return obj;
        }
        }
    internal class Program
    {
      /*  static string ObjectToString(Object obj) 
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
        }*/

        static void Main(string[] args)
        {
           /* TestClassForAttribute testClass = new TestClassForAttribute();
            string objString =ObjectToString(testClass);
            Console.WriteLine(objString);
            var obj =StringToObject(objString) as TestClassForAttribute;
            Console.WriteLine(obj);*/
            
            TestClassForAttribute testClass = new TestClassForAttribute { I=33};
            string serialized = CustomSerializer.ObjectToString(testClass);
            Console.WriteLine(serialized);

            TestClassForAttribute testClassDeserialized = (CustomSerializer.StringToObject<TestClassForAttribute>(serialized)) as TestClassForAttribute;
            int result = testClassDeserialized.I;
            Console.WriteLine(result);

        }
    }
}
