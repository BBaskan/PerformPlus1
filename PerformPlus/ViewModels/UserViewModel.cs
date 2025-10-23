    using PerformPlus.Models;
    using PerformPlus.Services;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;

    namespace PerformPlus.ViewModels
    {
        public class UserViewModel : INotifyPropertyChanged
        {
            private readonly int _employeeId; 

            public ObservableCollection<TaskAssignment> MyAssignments { get; }
              = new ObservableCollection<TaskAssignment>();

            private TaskAssignment _selectedAssignment;
            public TaskAssignment SelectedAssignment
            {
                get => _selectedAssignment;
                set
                {
                    _selectedAssignment = value;
                    OnPropertyChanged(nameof(SelectedAssignment));
                    LoadComments();
                }
            }

            public ObservableCollection<Comment> Comments { get; }
              = new ObservableCollection<Comment>();

            public ICommand CompleteTaskCommand { get; }
            public ICommand AddCommentCommand { get; }

            private string _newCommentText;
            public string NewCommentText
            {
                get => _newCommentText;
                set { _newCommentText = value; OnPropertyChanged(nameof(NewCommentText)); }
            }

            public UserViewModel(int employeeId)
            {
                _employeeId = employeeId;
                CompleteTaskCommand = new RelayCommand(_ => CompleteTask(), _ => SelectedAssignment != null);
                AddCommentCommand = new RelayCommand(_ => PostComment(), _ => !string.IsNullOrWhiteSpace(NewCommentText));

                LoadAssignments();
            }

            private void LoadAssignments()
            {
                MyAssignments.Clear();
                foreach (var ta in TaskService.GetAssignmentsForEmployee(_employeeId))
                    MyAssignments.Add(ta);
            }

            private void CompleteTask()
            {
                SelectedAssignment.Status = "Completed";
                SelectedAssignment.CompletedAt = DateTime.Now;
                TaskService.UpdateAssignment(SelectedAssignment);
                LoadAssignments();
            }

            private void LoadComments()
            {
                Comments.Clear();
                if (SelectedAssignment != null)
                    foreach (var c in CommentService.GetCommentsForTask(SelectedAssignment.TaskID))
                        Comments.Add(c);
            }

            private void PostComment()
            {
                var c = new Comment
                {
                    TaskID = SelectedAssignment.TaskID,
                    EmployeeID = _employeeId,
                    CommentText = NewCommentText,
                    CommentedAt = DateTime.Now
                };
                CommentService.AddComment(c);
                Comments.Add(c);
                NewCommentText = "";
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string n)
              => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
        }
    }
