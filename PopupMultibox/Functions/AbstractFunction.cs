using System;
using System.Collections.Generic;
using Multibox.Core.UI;

namespace Multibox.Core.Functions
{
    public abstract class AbstractFunction : IMultiboxFunction
    {
        public abstract int SuggestedIndex();
        public abstract bool Triggers(MultiboxFunctionParam args);

        public virtual bool IsMulti(MultiboxFunctionParam args)
        {
            return false;
        }

        public virtual bool IsBackgroundStream(MultiboxFunctionParam args)
        {
            return false;
        }

        public virtual bool ShouldRun(MultiboxFunctionParam args)
        {
            return true;
        }

        public virtual string RunSingle(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public virtual List<ResultItem> RunMulti(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public virtual void RunSingleBackgroundStream(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public virtual void RunMultiBackgroundStream(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public virtual bool HasDetails(MultiboxFunctionParam args)
        {
            return false;
        }

        public virtual bool IsBackgroundDetailsStream(MultiboxFunctionParam args)
        {
            return false;
        }

        public virtual string GetDetails(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public virtual void GetBackgroundDetailsStream(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public virtual bool HasActions(MultiboxFunctionParam args)
        {
            return false;
        }

        public virtual bool IsBackgroundActionsStream(MultiboxFunctionParam args)
        {
            return false;
        }

        public virtual List<ResultItem> GetActions(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public virtual void GetBackgroundActionsStream(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public virtual bool HasAction(MultiboxFunctionParam args)
        {
            return false;
        }

        public virtual void RunAction(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public virtual bool SupressKeyPress(MultiboxFunctionParam args)
        {
            return false;
        }

        public virtual bool HasKeyDownAction(MultiboxFunctionParam args)
        {
            return false;
        }

        public virtual void RunKeyDownAction(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public virtual bool HasActionKeyEvent(MultiboxFunctionParam args)
        {
            return false;
        }

        public virtual void RunActionKeyEvent(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public virtual bool HasSpecialDisplayCopyHandling(MultiboxFunctionParam args)
        {
            return false;
        }

        public virtual string RunSpecialDisplayCopyHandling(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public virtual bool HasSpecialInputCopyHandling(MultiboxFunctionParam args)
        {
            return false;
        }

        public virtual string RunSpecialInputCopyHandling(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }
    }
}