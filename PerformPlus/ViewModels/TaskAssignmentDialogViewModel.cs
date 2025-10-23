using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using PerformPlus.Models;
using PerformPlus.Services;

namespace PerformPlus.ViewModels
{
    public class TaskAssignmentDialogViewModel : INotifyPropertyChanged
    {
        public TaskAssignment Assignment { get; }
        public ObservableCollection<EmployeeModel> Employees { get; } = new();
        public ObservableCollection<EmployeeModel> SelectedEmployees { get; } = new();

        public ObservableCollection<Comment> Comments { get; } = new();

        public List<TaskAssignment> CreatedAssignments { get; } = new();


        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddCommentCommand { get; }

        private string _newCommentText;
        public string NewCommentText
        {
            get => _newCommentText;
            set
            {
                _newCommentText = value;
                OnPropertyChanged(nameof(NewCommentText));
            }
        }

        public event EventHandler<bool> RequestClose;

        public TaskAssignmentDialogViewModel(TaskAssignment assignment)
        {
            Assignment = assignment;

            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => Cancel());
            AddCommentCommand = new RelayCommand(_ => AddComment(), _ => !string.IsNullOrWhiteSpace(NewCommentText));

            LoadEmployees();
            LoadComments();
        }

        private void LoadEmployees()
        {
            Employees.Clear();
            foreach (var emp in UserService.GetAll())
                Employees.Add(emp);
        }

        private void LoadComments()
        {
            Comments.Clear();
            if (Assignment.TaskID == 0) return;

            foreach (var comment in CommentService.GetCommentsForTask(Assignment.TaskID))
                Comments.Add(comment);
        }

        private void AddComment()
        {
            var comment = new Comment
            {
                TaskID = Assignment.TaskID,
                EmployeeID = SessionManager.EmployeeID,
                CommentText = NewCommentText,
                CommentedAt = DateTime.Now
            };

            CommentService.AddComment(comment);
            Comments.Add(comment);
            NewCommentText = string.Empty;
        }

        private void Save()
        {
            if (Assignment.TaskID == 0)
            {
                Assignment.TaskID = TaskService.CreateTaskAndGetId(
                    Assignment.Title,
                    Assignment.Description,
                    Assignment.DueDate,
                    Assignment.AssignedAt
                );
            }

            foreach (var employee in SelectedEmployees)
            {
                var taskAssignment = new TaskAssignment
                {
                    TaskID = Assignment.TaskID,
                    EmployeeID = employee.EmployeeID,
                    AssignedAt = Assignment.AssignedAt,
                    AssignedBy = SessionManager.EmployeeID,
                    Status = Assignment.Status
                };

                CreatedAssignments.Add(taskAssignment); 
                TaskService.AssignTask(taskAssignment);
            }

            RequestClose?.Invoke(this, true);
        }



        private void Cancel() => RequestClose?.Invoke(this, false);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}