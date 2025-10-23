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
using PerformPlus.Views;
using PerformPlus.ViewModels;


namespace PerformPlus.ViewModels
{
    public class LeaderViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<OvertimeEntry> OvertimeEntries { get; } = new();
        public ObservableCollection<TaskAssignment> TaskAssignments { get; } = new();
        public ObservableCollection<Comment> TaskComments { get; } = new();

        private TaskAssignment _selectedTask;
        public TaskAssignment SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
                LoadComments();
            }
        }

        private OvertimeEntry _selectedOvertime;
        public OvertimeEntry SelectedOvertime
        {
            get => _selectedOvertime;
            set { _selectedOvertime = value; OnPropertyChanged(nameof(SelectedOvertime)); }
        }

        private readonly int _leaderId = 1;

        // Commands
        public ICommand AddOvertimeCommand { get; }
        public ICommand ApproveOvertimeCommand { get; }

        public ICommand AddTaskCommand { get; }
        public ICommand EditTaskCommand { get; }
        public ICommand ApproveTaskCommand { get; }

        public LeaderViewModel()
        {
            AddOvertimeCommand = new RelayCommand(_ => AddOvertime());
            ApproveOvertimeCommand = new RelayCommand(_ => ApproveOvertime(), _ => SelectedOvertime != null);

            AddTaskCommand = new RelayCommand(_ => AddTask());
            EditTaskCommand = new RelayCommand(_ => EditTask(), _ => SelectedTask != null);
            ApproveTaskCommand = new RelayCommand(_ => ApproveTask(), _ => SelectedTask != null);

            LoadOvertimeEntries();
            LoadTasks();
        }

        private void LoadOvertimeEntries()
        {
            OvertimeEntries.Clear();
            foreach (var e in OvertimeService.GetRecentEntries(_leaderId))
                OvertimeEntries.Add(e);
        }

        public void AddOvertime()
        {
            var dlg = new OvertimeEntryDialog { Owner = App.Current.MainWindow };
            if (dlg.ShowDialog() == true)
            {
                dlg.Entry.LeaderID = _leaderId;
                OvertimeService.AddEntry(dlg.Entry);
                LoadOvertimeEntries();
            }
        }

        public void ApproveOvertime()
        {
            OvertimeService.ApproveEntry(SelectedOvertime.EntryID, _leaderId);
            LoadOvertimeEntries();
        }

        private void LoadTasks()
        {
            TaskAssignments.Clear();
            foreach (var t in TaskService.GetAssignmentsForLeader(_leaderId))
                TaskAssignments.Add(t);
        }

        public void AddTask()
        {
            var dlg = new TaskAssignmentDialog { Owner = App.Current.MainWindow };
            if (dlg.ShowDialog() == true)
            {
                foreach (var assignment in dlg.ViewModel.CreatedAssignments)
                {
                    TaskAssignments.Add(assignment); 
                }

                LoadTasks(); 
            }
        }


        public void EditTask()
        {
            var dlg = new TaskAssignmentDialog(SelectedTask) { Owner = App.Current.MainWindow };
            if (dlg.ShowDialog() == true)
            {
                TaskService.UpdateAssignment(dlg.ViewModel.Assignment);

                LoadTasks();
            }
        }

        public void ApproveTask()
        {
            SelectedTask.ApprovedAt = DateTime.Now;
            SelectedTask.Status = "Approved";
            TaskService.UpdateAssignment(SelectedTask);
            LoadTasks();
        }

        private void LoadComments()
        {
            TaskComments.Clear();
            if (SelectedTask != null)
            {
                foreach (var c in CommentService.GetCommentsForTask(SelectedTask.TaskID))
                    TaskComments.Add(c);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
