
using System;
using Domain.ComponentManagement;

namespace Domain.Models
{
    public class SubscribedComponentResultModel
    {
        private SubscribedComponentResult ComponentResult { get; }

        public SubscribedComponentResultModel(
            SubscribedComponentResult componentResult)
        {
            if (componentResult == SubscribedComponentResult.Failed)
            {
                throw new InvalidOperationException("Use constructor with error message");
            }
            ComponentResult = componentResult;
        }

        public SubscribedComponentResultModel(string error)
        {
            ComponentResult = SubscribedComponentResult.Failed;
            Error = error;
        }


        public string Error { get; }

        public static SubscribedComponentResultModel AlreadyExists()
        {
            return new SubscribedComponentResultModel(
                SubscribedComponentResult.AlreadyExists);
        }

        public static SubscribedComponentResultModel Failed(string error)
        {
            return new SubscribedComponentResultModel(error);
        }

        public static SubscribedComponentResultModel Successful()
        {
            return new SubscribedComponentResultModel(
                SubscribedComponentResult.Successful);
        }
    }
}