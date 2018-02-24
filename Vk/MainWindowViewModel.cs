using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using JetBrains.Annotations;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace Vk
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public ICommand LoginCommand { get; }
        public ICommand RefreshCommand { get; }
        private VkApi _api;
        private readonly string[] _forbiddenWords = {"дыбенко", "мега", "девяткино", "парнас", "кудрово","большевиков", "коттедж", "двухкомнатн",
            "купчино", "лесная", "гражданск", "академическ", "транспорт", "маршрутк", "ладожск"};

        public ObservableCollection<BoardItemViewModel> BoardItems { get; } = new ObservableCollection<BoardItemViewModel>();
        private readonly HashSet<long> _existingItems = new HashSet<long>();
        private readonly HashSet<string> _existingComments = new HashSet<string>();

        private readonly long _groupId = 12469956;
        private readonly long _boardId = 33400181;

        public MainWindowViewModel()
        {
            LoginCommand = new RelayCommand(LoginAction, LoginRequired);
            RefreshCommand = new RelayCommand(RefreshAction, CanRefresh);

            var timer = new DispatcherTimer {Interval = TimeSpan.FromMinutes(10)};
            timer.Tick += Refresh;
            timer.Start();
        }

        private void Refresh(object sender, EventArgs e)
        {
            RefreshAction();
        }

        private bool CanRefresh()
        {
            return !LoginRequired();
        }

        private void RefreshAction()
        {
            LoadItems(10);
        }

        private bool LoginRequired()
        {
            return _api == null;
        }

        private void LoginAction()
        {
            var auth = new Authorizaion();

            if (auth.ShowDialog().Value)
            {
                var token = auth.Token;
                try
                {
                    _api = new VkApi();

                    _api.Authorize(new ApiAuthParams
                    {
                        AccessToken = token,
                        Settings = Settings.All
                    });
                    LoadItems(100);
                }
                catch (Exception)
                {
                    MessageBox.Show("Authorization failed");
                    _api = null;
                }
            }
        }



        private void LoadItems(int count)
        {
            if (_api == null)
            {
                return;
            }

            var c = _api.Board.GetComments(new BoardGetCommentsParams() { TopicId = _boardId, GroupId = _groupId, Count = count, Sort = CommentsSort.Desc });

            foreach (var comment in c.Items.Reverse())
            {
                if (PassesFilter(comment))
                {
                    BoardItems.Add(new BoardItemViewModel(comment.Id.ToString(), comment.Text, _boardId,
                        _groupId));
                    _existingItems.Add(comment.Id);
                    _existingComments.Add(comment.Text.Trim().Replace("\n", ""));
                }
            }
        }



        private bool PassesFilter(Comment comment)
        {
            if (_existingItems.Contains(comment.Id))
                return false;

            if (comment.Attachments.Count == 0)
                return false;

            if (ContainsForbiddenWords(comment.Text))
                return false;

            if (_existingComments.Contains(comment.Text.Trim().Replace("\n", "")))
            {
                return false;
            }

            return true;
        }

        private bool ContainsForbiddenWords(string commentText)
        {
            foreach (var forbiddenWord in _forbiddenWords)
            {
                if (commentText.IndexOf(forbiddenWord, StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    return true;
                }
            }

            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
