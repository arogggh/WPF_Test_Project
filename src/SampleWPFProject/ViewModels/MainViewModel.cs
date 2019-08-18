﻿using System.Data.Entity;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BLL.Services.Interfaces;
using BLL.Services;
using BLL.Models;
using WPFProject.Commands;
using System.Collections.Generic;
using System;

namespace WPFProject.ViewModels
{
    /// <summary>
    /// Class contains all ViewModels and commands for working with them.
    /// </summary>
    public class MainViewModel
    {
        private readonly IContentBaseService contentBaseService;

        public TreeViewModel TreeViewModel { get; }

        public ListViewModel ListViewModel { get; }

        public TextBlockViewModel TextBlockViewModel { get; }

        public MainViewModel()
        {
            contentBaseService = new ContentBaseService();

            TreeViewModel = new TreeViewModel();
            TreeViewModel.FoldersList = GetFoldersTree();
           
            ListViewModel = new ListViewModel(contentBaseService);
            TextBlockViewModel = new TextBlockViewModel();
        }

        public ICommand SaveItem
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    var name = TextBlockViewModel.Name;
                    var description = TextBlockViewModel.Description;
                    var editableItem = TextBlockViewModel.EditableItem;

                    if (!string.Equals(name, editableItem.Name) || !string.Equals(description, editableItem.Description))
                    {
                        editableItem.Name = name;
                        editableItem.Description = description;

                        contentBaseService.Update(editableItem);

                        TextBlockViewModel.Name = string.Empty;
                        TextBlockViewModel.Description = string.Empty;

                        TreeViewModel.FoldersList = GetFoldersTree();
                    }
                });
            }
        }

        public ICommand EditItem
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    var content = ListViewModel.SelectedItem;

                    if (content != null)
                    {
                        TextBlockViewModel.Name = content.Name;
                        TextBlockViewModel.Description = content.Description;

                        TextBlockViewModel.EditableItem = content;
                    }
                });
            }
        }

        private ObservableCollection<ContentBaseModel> GetFoldersTree()
        {
            var collection = contentBaseService.GetContentItemsList()
                                               .Where(x => x.GetType() == typeof(ContentFolderModel) && x.ParentContentItem == null)
                                               .Select(i=> 
                                               {
                                                   i.Children = new ObservableCollection<ContentBaseModel>(Flatten(i, x => x.Children.Where(y => y.GetType() == typeof(ContentFolderModel))));
                                                   return i;
                                               });

            return new ObservableCollection<ContentBaseModel>(collection);
        }

        public IEnumerable<ContentBaseModel> Flatten(ContentBaseModel source, Func<ContentBaseModel, IEnumerable<ContentBaseModel>> selector)
        {
            return selector(source).Select(c =>
            {
                c.Children = new ObservableCollection<ContentBaseModel>(Flatten(c, selector));
                return c;
            });
        }
    }
}