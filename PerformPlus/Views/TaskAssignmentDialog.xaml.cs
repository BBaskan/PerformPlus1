using PerformPlus.Models;
using PerformPlus.Services;
using PerformPlus.ViewModels;
using System;
using System.Windows;

namespace PerformPlus.Views
{
    public partial class TaskAssignmentDialog : Window
    {
        private readonly TaskAssignmentDialogViewModel viewModel;

        
        public TaskAssignmentDialogViewModel ViewModel => viewModel;

        public TaskAssignmentDialog()
        {
            InitializeComponent();
            var assignment = new TaskAssignment { AssignedAt = DateTime.Now, Status = "Pending" };
            viewModel = new TaskAssignmentDialogViewModel(assignment);
            viewModel.RequestClose += (s, result) => DialogResult = result;
            DataContext = viewModel;
        }

        public TaskAssignmentDialog(TaskAssignment existingAssignment)
        {
            InitializeComponent();
            viewModel = new TaskAssignmentDialogViewModel(existingAssignment);
            viewModel.RequestClose += (s, result) => DialogResult = result;
            DataContext = viewModel;
        }
    }
}
