using System;

namespace QuickGameFramework.Runtime.UI {
    public struct Message {
        public enum CommonCommand {
            Show, Hide, Refresh
        }

        public ValueType Command;
        public ValueType ExtraParams;
    }
}
