﻿using BLL.Models;

namespace WPFProject.ViewModels
{
    public class TextBlockViewModel : NotifyPropertyChanged
    {
        private string name;

        private string description;

        public ContentBaseModel EditableItem { get; set; }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }
    }
}
