using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TryXamarinApp.Classes
{
    //public class Person
    //{
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public Guid Id { get; set; }
    //    public string Name { get { return FirstName + " " + LastName; } }

    //    public string PersonGroup { get; set; }
    //    public Person(string first, string last)
    //    {
    //        this.FirstName = first;
    //        this.LastName = last;
    //        this.Id = Guid.NewGuid();
    //        this.PersonGroup = "trypersongroupid";
    //    }
    //    public Person(string first, string last, Guid id, string personGroupId= "trypersongroupid")
    //    {
    //        this.FirstName = first;
    //        this.LastName = last;
    //        this.Id = id;
    //        this.PersonGroup = personGroupId;
    //    }
    //}

    public class Person :IComparable
    {
        public string Name { get; set; }
        public Guid PersonId { get; set; }
        public IList<Guid> PersistedFaceIds { get; set; }
        public UserDataType UserData { get; set; }
        public string Password { get; set; }
        //public ImageSource Pic { get; set; }
        public string FullName { get { return UserData.firstName + " " + UserData.lastName; } }
        public Person(string name, Guid guid, UserDataType userData = null, IList<Guid> persistedFaceIds = null)
        {
            Name = name;
            PersonId = guid;
            UserData = userData;
            PersistedFaceIds = persistedFaceIds;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Person otherPerson = obj as Person;
            if (otherPerson != null)
            {
                if (this.UserData.firstName.Equals(otherPerson.UserData.firstName))
                {
                    return this.UserData.lastName.CompareTo(otherPerson.UserData.lastName);
                }
                return this.UserData.firstName.CompareTo(otherPerson.UserData.firstName);
            }
            else
                throw new ArgumentException("Object is not a Person");
        }
    }

    public class UserDataType
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public UserDataType()
        {
            firstName = null;
            lastName = null;
        }
    }
}

