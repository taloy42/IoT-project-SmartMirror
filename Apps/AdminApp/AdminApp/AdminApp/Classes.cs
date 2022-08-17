using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AdminApp
{
    public class Admin
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class PersonGroup
    {
        public string PersonGroupId
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }

        public string UserData
        {
            get;
            set;
        }
        public string RecognitionModel
        {
            get;
            set;
        }
        public IList<Person> Persons { get; set; }
    }
    public class Person : IComparable
    {
        public string Name { get; set; }
        public Guid PersonId { get; set; }
        public IList<Guid> PersistedFaceIds { get; set; }
        public UserDataType UserData { get; set; }
        public string Password { get; set; }
        //public ImageSource Pic { get; set; }
        public string FullName { get { return UserData.firstName + " " + UserData.lastName; } }

        public DataTemplate PersonDataTemplate { get {
                return new DataTemplate(() =>
                {
                    Label nameLabel = new Label();
                    nameLabel.SetBinding(Label.TextProperty, new Binding("FullName", BindingMode.OneWay, null, null, "Name: {0}"));

                    Label usernameLabel = new Label();
                    usernameLabel.SetBinding(Label.TextProperty, new Binding("Name", BindingMode.OneWay, null, null, "Username: {0}"));

                    Label idLabel = new Label();
                    idLabel.SetBinding(Label.TextProperty, new Binding("PersonId", BindingMode.OneWay, null, null, "Guid: {0}"));


                    var final = new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        Children =
                    {
                        nameLabel,
                        usernameLabel,
                        idLabel
                    }
                    };



                    ViewCell cell = new ViewCell
                    {
                        View = final
                    };

                    return cell;
                });
            } }
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
