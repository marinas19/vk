using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Vk
{
    class BoardItemViewModel :INotifyPropertyChanged
    {
        public ICommand MouseEnterCommand { get; }
        public ICommand ShowCommand { get; }
        public ICommand ShowPhotosCommand { get; }
        public string Comment { get; }
        public bool Viewed
        {
            get => _viewed;
            set {
                if (_viewed != value)
                {
                    _viewed = value;
                    OnPropertyChanged();
                }
            }
        }
        private readonly string _commentId;
        private readonly long _groupId;
        private readonly long _boardId;
        private bool _viewed;

        public BoardItemViewModel(string commentId, string comment, long boardId, long groupId)
        {
            _commentId = commentId;
            _boardId = boardId;
            _groupId = groupId;
            Comment = comment;
            ShowCommand = new RelayCommand(ShowAction);
            ShowPhotosCommand = new RelayCommand(ShowPhotosAction);
            MouseEnterCommand = new RelayCommand(MouseEnterAction);
        }

        private void MouseEnterAction()
        {
            Viewed = true;
        }


        private void ShowAction()
        {
            Process.Start("chrome.exe", $"https://vk.com/topic-{_groupId}_{_boardId}?post={_commentId}");
        }

        private void ShowPhotosAction()
        {
            Process.Start("chrome.exe", $"https://vk.com/topic?act=browse_images&id=-{_groupId}_{_commentId}");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
