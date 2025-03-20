import { ChatMessage } from "../../models/ChatMessage";
import { CommonAction, CommonState } from "./commonTypes";

export const CHAT_CREATE_THREAD_REQUEST = "@CHAT/CREATE_THREAD_REQUEST";
export const CHAT_CREATE_THREAD_SUCCESS = "@CHAT/CREATE_THREAD_SUCCESS";
export const CHAT_CREATE_THREAD_FAILURE = "@CHAT/CREATE_THREAD_FAILURE";

export const CHAT_ADD_MESSAGE_REQUEST = "@CHAT/ADD_MESSAGE_REQUEST";
export const CHAT_ADD_MESSAGE_SUCCESS = "@CHAT/ADD_MESSAGE_SUCCESS";
export const CHAT_ADD_MESSAGE_FAILURE = "@CHAT/ADD_MESSAGE_FAILURE";

export const CHAT_FETCH_FRESH_MESSAGES_REQUEST = "@CHAT/FETCH_FRESH_MESSAGES_REQUEST";
export const CHAT_FETCH_FRESH_MESSAGES_SUCCESS = "@CHAT/FETCH_FRESH_MESSAGES_SUCCESS";
export const CHAT_FETCH_FRESH_MESSAGES_FAILURE = "@CHAT/FETCH_FRESH_MESSAGES_FAILURE";

export type ChatState = CommonState & {
  threadId?: string;
  history: Array<ChatMessage>;
};

type ChatCreateThreadRequestAction = {
  type: typeof CHAT_CREATE_THREAD_REQUEST;
};

type ChatCreateThreadSuccessAction = {
  type: typeof CHAT_CREATE_THREAD_SUCCESS;
  threadId: string;
};

type ChatCreateThreadFailureAction = {
  type: typeof CHAT_CREATE_THREAD_FAILURE;
  error: string;
};

type ChatAddMessageRequestAction = {
  type: typeof CHAT_ADD_MESSAGE_REQUEST;
};

type ChatAddMessageSuccessAction = {
  type: typeof CHAT_ADD_MESSAGE_SUCCESS;
};

type ChatAddMessageFailureAction = {
  type: typeof CHAT_ADD_MESSAGE_FAILURE;
  error: string;
};

type ChatFetchFreshMessagesRequestAction = {
  type: typeof CHAT_FETCH_FRESH_MESSAGES_REQUEST;
};

type ChatFetchFreshMessagesSuccessAction = {
  type: typeof CHAT_FETCH_FRESH_MESSAGES_SUCCESS;
  messages: Array<ChatMessage>;
};

type ChatFetchFreshMessagesFailureAction = {
  type: typeof CHAT_FETCH_FRESH_MESSAGES_FAILURE;
  error: string;
};

export type ChatAction = CommonAction
  | ChatCreateThreadRequestAction
  | ChatCreateThreadSuccessAction
  | ChatCreateThreadFailureAction
  | ChatAddMessageRequestAction
  | ChatAddMessageSuccessAction
  | ChatAddMessageFailureAction
  | ChatFetchFreshMessagesRequestAction
  | ChatFetchFreshMessagesSuccessAction
  | ChatFetchFreshMessagesFailureAction;
