namespace Chat{
    public static class Protocolo{
        public enum MessageType{

            IDENTIFY,
            STATUS,
            USERS,
            TEXT,
            PUBLIC_TEXT,
            NEW_USER,
            NEW_STATUS,
            USER_LIST,
            TEXT_FROM,
            PUBLIC_TEXT_FROM,
            RESPONSE
        }
        public enum UserStatus{

            ACTIVE,
            AWAY,
            BUSY,

        }
        public enum OperationResult{
            SUCCESS,
            USER_ALREADY_EXISTS,
            NO_SUCH_USER,
            INVALID
        }
        public enum OperationType{

            IDENTIFY,
            STATUS,
            USERS,
            TEXT,
            PUBLIC_TEXT,
            INVALID,
        }
    }
}
