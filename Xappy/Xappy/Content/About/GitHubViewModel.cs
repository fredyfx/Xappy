﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Octokit;
using Xamarin.Forms;

namespace Xappy.About.ViewModels
{
    public class GitHubViewModel : INotifyPropertyChanged //: BaseViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly GitHubClient _xappyGitHubClient = new GitHubClient(new ProductHeaderValue("Xappy"));

        private ObservableCollection<RepositoryContributor> _contributors;
        public ObservableCollection<RepositoryContributor> Contributors
        {
            get => _contributors;
            set
            {
                _contributors = value;
                OnPropertyChanged();

                //SetAndRaisePropertyChanged(ref _contributors, value);
            }
        }

        private RepositoryContributor _selectedContributor;
        public RepositoryContributor SelectedContributor
        {
            get => _selectedContributor;
            set
            {
                _selectedContributor = value;

                if (_selectedContributor != null)
                {
                    OnPropertyChanged();

                    if (!string.IsNullOrEmpty(SelectedContributor.HtmlUrl))
                        Device.OpenUri(new Uri(SelectedContributor.HtmlUrl));

                    //Clear current selection
                    SelectedContributor = null;
                }
            }
        }

        public async Task OnAppearing()
        {
            //TODO: Depechie - Add loading animation?
            //TODO: Depechie - Implement pull to refresh?
            try
            {
                if (Contributors == null)
                {
                    var contributors = await _xappyGitHubClient.Repository.GetAllContributors("davidortinau", "Xappy");
                    if (contributors != null)
                    {
                        //Initiate poor mans randomizer for lists
                        //Note: there are better options for real production worthy large lists : https://stackoverflow.com/questions/273313/randomize-a-listt
                        //But for now this linq version will do
                        Random rnd = new Random();
                        Contributors = new ObservableCollection<RepositoryContributor>(contributors.Select(x => new { value = x, order = rnd.Next() }).OrderBy(x => x.order).Select(x => x.value));
                    }
                }
            }
            catch(Exception ex)
            {
            }
        }
    }
}