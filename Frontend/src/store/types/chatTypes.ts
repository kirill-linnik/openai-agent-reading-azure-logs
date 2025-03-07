import { ChatMessage } from "../../models/ChatMessage";
import { CommonState } from "./commonTypes";

export const CHAT_ADD_MESSAGE_REQUEST = "@CHAT/ADD_MESSAGE_REQUEST";
export const CHAT_ADD_MESSAGE_SUCCESS = "@CHAT/ADD_MESSAGE_SUCCESS";
export const CHAT_ADD_MESSAGE_FAILURE = "@CHAT/ADD_MESSAGE_FAILURE";

export type ChatState = CommonState & {
  history: Array<ChatMessage>;
};

type ChatAddMessageRequestAction = {
  type: typeof CHAT_ADD_MESSAGE_REQUEST;
  message: string;
};

type ChatAddMessageSuccessAction = {
  type: typeof CHAT_ADD_MESSAGE_SUCCESS;
  message: ChatMessage;
};

type ChatAddMessageFailureAction = {
  type: typeof CHAT_ADD_MESSAGE_FAILURE;
  error: string;
};

export type ChatAction =
  | ChatAddMessageRequestAction
  | ChatAddMessageSuccessAction
  | ChatAddMessageFailureAction;
