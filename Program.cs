using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRepository
{
    public class Person
    {
        public Person(int id, string name, string surName, int age)
        {
            Id = id;
            Name = name;
            SurName = surName;
            Age = age;
        }
        public Person()
        {

        }
        private int _age;

        public int Id { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public int Age
        {
            get
            {
                return _age;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("Age must be an integer than 0");
                }
                _age = value;
            }
        }

        public interface IPersonRepository
        {
            void AddPerson(Person person);
            void DeletePerson(int id);
            void UpdatePerson(Person person);
            Person GetById(int id);

            List<Person> GetAll();

        }

        public class PersonRepository : IPersonRepository
        {

            public const string CONNECTION_STRING = "Data Source=LUSINE1985\\MSSQLSERVER01;Initial Catalog=PeopleDb;Integrated Security=True;Encrypt=False";
            List<Person> people = new List<Person>();
            public void AddPerson(Person person)
            {
                using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = " insert into Persons values(@Id,@Name,@SurName,@Age)";
                        command.Parameters.Add(new SqlParameter("@Id", person.Id));
                        command.Parameters.Add(new SqlParameter("@Name", person.Name));
                        command.Parameters.Add(new SqlParameter("@SurName", person.SurName));
                        command.Parameters.Add(new SqlParameter("@Age", person.Age));
                        command.ExecuteNonQuery();
                    }
                }
            }

            public void DeletePerson(int id)
            {
                using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "Delete from Persons where Id=@Id";
                        command.Parameters.Add(new SqlParameter("Id", id));
                        command.ExecuteNonQuery();
                    }
                }
            }

            public List<Person> GetAll()
            {
                using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "select * from Persons";
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Person person = GetById(int.Parse(reader["Id"].ToString()));
                                people.Add(person);

                            }
                        }
                    }
                    return people;
                }
            }

            public Person GetById(int id)
            {
                using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
                {
                    connection.Open();
                    Person person = new Person();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "select * from Persons where Id=@Id";
                        command.Parameters.Add(new SqlParameter("@Id", id));
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                person.Id = int.Parse(reader["Id"].ToString());
                                person.Name = reader["Name"].ToString();
                                person.SurName = reader["SurName"].ToString();
                                person.Age = int.Parse(reader["Age"].ToString());
                            }
                        }
                    }
                    return person;
                }
            }

            public void UpdatePerson(Person person)
            {
                using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "update Persons set Name=@Name, SurName=@SurName,Age=@Age where Id=@Id";
                        command.Parameters.Add(new SqlParameter("@Id", person.Id));
                        command.Parameters.Add(new SqlParameter("@Name", person.Name));
                        command.Parameters.Add(new SqlParameter("@SurName", person.SurName));
                        command.Parameters.Add(new SqlParameter("@Age", person.Age));
                        command.ExecuteNonQuery();
                    }
                }
            }
        }


        internal class Program
        {

            public static Person PrintDetails()
            {
                Console.Write("Type Id: ");
                int id = int.Parse(Console.ReadLine());
                Console.Write("Type Name: ");
                string name = (Console.ReadLine());
                Console.Write("Type SurName: ");

                string surName = Console.ReadLine();

                Console.Write("Type Age: ");

                int age = int.Parse(Console.ReadLine());

                Person person = new Person(id, name, surName, age);
                return person;

            }
            static void Main(string[] args)
            {
                int choice;
                do
                {
                    Console.WriteLine("1-AddPerson | 2-DeletePerson | 3-UpdatePerson | 4-GetAll | 0-Exit");
                    Console.Write("Your choice: ");
                    choice = int.Parse(Console.ReadLine());
                    IPersonRepository personRepository = new PersonRepository();
                    switch (choice)
                    {
                        case 1:
                            {
                                Person newPerson = PrintDetails();
                                personRepository.AddPerson(newPerson);
                                Console.WriteLine("Data added to table Persons");
                                Console.WriteLine("\n");
                            };
                            break;
                        case 2:
                            {
                                Console.Write("Type Id: ");
                                int id = int.Parse(Console.ReadLine());
                                personRepository.DeletePerson(id);
                                Console.WriteLine("Data deleted from table Persons");
                                Console.WriteLine("\n");



                            };
                            break;
                        case 3:
                            {
                                Person newPerson = PrintDetails();
                                personRepository.UpdatePerson(newPerson);
                                Console.WriteLine("Data updated in table Persons");
                                Console.WriteLine("\n");


                            };
                            break;
                        case 4:
                            {
                                var people = personRepository.GetAll();
                                Console.WriteLine("All data from table Persons");
                                foreach (Person person in people)
                                {
                                    Console.WriteLine($"Id: {person.Id}, Name: {person.Name}, SurName:{person.SurName}, Age: {person.Age}");

                                }
                                Console.WriteLine("\n");

                            };
                            break;
                        case 0:
                            Console.WriteLine("End programm");
                            break;
                        default:
                            Console.WriteLine("Invalid choice");
                            break;

                    }

                } while (choice > 0);



                Console.ReadLine();
            }
        }
    }
}
