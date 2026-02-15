using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagement.Shared.Constants
{
    public static class AppConstants
    {
        public static class ValidationMessages
        {
            public const string FieldRequired = "{0} is required";
            public const string FieldMinLength = "{0} must be at least {1} characters";
            public const string FieldMaxLength = "{0} must not exceed {1} characters";
            public const string InvalidEmail = "Invalid email format";
            public const string InvalidPhoneNumber = "Invalid phone number format";
            public const string DateInPast = "Date cannot be in the past";
            public const string DueDateMustBeGreaterThanStartDate = "Due date must be greater than start date";
        }

        public static class ErrorMessages
        {
            public const string InternalServerError = "An internal server error occurred";
            public const string NotFound = "{0} not found";
            public const string Unauthorized = "You are not authorized to perform this action";
            public const string Forbidden = "You do not have permission to access this resource";
            public const string InvalidCredentials = "Invalid username or password";
            public const string UserAlreadyExists = "User with this email already exists";
            public const string ProjectNotFound = "Project not found";
            public const string TaskNotFound = "Task not found";
            public const string UserNotFound = "User not found";
        }

        public static class SuccessMessages
        {
            public const string RecordCreated = "{0} created successfully";
            public const string RecordUpdated = "{0} updated successfully";
            public const string RecordDeleted = "{0} deleted successfully";
            public const string OperationCompleted = "Operation completed successfully";
        }

        public static class Pagination
        {
            public const int DefaultPageNumber = 1;
            public const int DefaultPageSize = 10;
            public const int MaxPageSize = 100;
        }

        public static class CacheKeys
        {
            public const string UsersKey = "users_{0}";
            public const string ProjectsKey = "projects_{0}";
            public const string TasksKey = "tasks_{0}";
        }

        public static class Roles
        {
            public const string Admin = "Admin";
            public const string ProjectManager = "ProjectManager";
            public const string TeamLead = "TeamLead";
            public const string Developer = "Developer";
            public const string Viewer = "Viewer";
        }
    }
}
