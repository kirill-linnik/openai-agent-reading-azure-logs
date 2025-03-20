import { Dispatch } from "redux";
import { ChatMessage } from "../../models/ChatMessage";
import { ChatRole } from "../../models/ChatRole";
import dayjs from "dayjs";
import { backendProvider, extractError } from "../../services/BackendProvider";
import {
  CHAT_ADD_MESSAGE_FAILURE,
  CHAT_ADD_MESSAGE_REQUEST,
  CHAT_ADD_MESSAGE_SUCCESS,
  CHAT_CREATE_THREAD_FAILURE,
  CHAT_CREATE_THREAD_REQUEST,
  CHAT_CREATE_THREAD_SUCCESS,
  CHAT_FETCH_FRESH_MESSAGES_FAILURE,
  CHAT_FETCH_FRESH_MESSAGES_REQUEST,
  CHAT_FETCH_FRESH_MESSAGES_SUCCESS,
  ChatAction,
} from "../types/chatTypes";

function chatCreateThreadRequest(): ChatAction {
  return {
    type: CHAT_CREATE_THREAD_REQUEST
  };
}

function chatCreateThreadSuccess(threadId: string): ChatAction {
  return {
    type: CHAT_CREATE_THREAD_SUCCESS,
    threadId
  };
}

function chatCreateThreadFailure(error: string): ChatAction {
  return {
    type: CHAT_CREATE_THREAD_FAILURE,
    error,
  };
}

function chatAddMessageRequest(): ChatAction {
  return {
    type: CHAT_ADD_MESSAGE_REQUEST
  };
}

function chatAddMessageSuccess(): ChatAction {
  return {
    type: CHAT_ADD_MESSAGE_SUCCESS
  };
}

function chatAddMessageFailure(error: string): ChatAction {
  return {
    type: CHAT_ADD_MESSAGE_FAILURE,
    error,
  };
}

function chatFetchFreshMessagesRequest(): ChatAction {
  return {
    type: CHAT_FETCH_FRESH_MESSAGES_REQUEST
  };
}

function chatFetchFreshMessagesSuccess(messages: Array<ChatMessage>): ChatAction {
  return {
    type: CHAT_FETCH_FRESH_MESSAGES_SUCCESS,
    messages
  };
}

function chatFetchFreshMessagesFailure(error: string): ChatAction {
  return {
    type: CHAT_FETCH_FRESH_MESSAGES_FAILURE,
    error,
  };
}

export function createChatThread() {
  return async (dispatch: Dispatch<ChatAction>) => {
    dispatch(chatCreateThreadRequest());
    try {
      const response = await backendProvider.put("/chat");
      dispatch(chatCreateThreadSuccess(response.data));
    } catch (error) {
      dispatch(chatCreateThreadFailure(extractError(error)));
    }
  };
}

export function addChatMessage(
  threadId: string,
  newMessage: string
) {
  return async (dispatch: Dispatch<ChatAction>) => {
    dispatch(chatAddMessageRequest());
    try {
      const chatRequest = {
        threadId: threadId,
        message: newMessage
      };
      const response = await backendProvider.post("/chat", chatRequest);
      dispatch(chatAddMessageSuccess());
    } catch (error) {
      dispatch(chatAddMessageFailure(extractError(error)));
    }
  };
}

export function fetchFreshMessages(threadId: string, lastMessageId: string | undefined) {
  return async (dispatch: Dispatch<ChatAction>) => {
    dispatch(chatFetchFreshMessagesRequest());
    try {
      const safeThreadId = encodeURIComponent(threadId);
      const safeLastMessageId = lastMessageId ? encodeURIComponent(lastMessageId) : "";
      const response = await backendProvider.get(`/chat/${safeThreadId}?lastMessageId=${safeLastMessageId}`);
      const messages: Array<ChatMessage> = response.data.map(objToChatMessage);
      dispatch(chatFetchFreshMessagesSuccess(messages));
    } catch (error) {
      dispatch(chatFetchFreshMessagesFailure(extractError(error)));
    }
  };
}

function objToChatMessage(obj: any): ChatMessage {
  return {
    ...obj,
    role: ChatRole[obj.role as keyof typeof ChatRole],
    createdOn: dayjs(obj.createdOn, { utc: true }),
  };
}