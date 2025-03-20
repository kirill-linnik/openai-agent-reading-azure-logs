import { ChatMessage } from "../../models/ChatMessage";
import {
  CHAT_ADD_MESSAGE_FAILURE,
  CHAT_CREATE_THREAD_FAILURE,
  CHAT_CREATE_THREAD_SUCCESS,
  CHAT_FETCH_FRESH_MESSAGES_FAILURE,
  CHAT_FETCH_FRESH_MESSAGES_SUCCESS,
  ChatState,
} from "../types/chatTypes";
import commonReducer from "./commonReducer";

const initialState: ChatState = {
  history: [],
};

const chatReducer = (state = initialState, action: any): ChatState => {
  switch (action.type) {
    case CHAT_CREATE_THREAD_SUCCESS:
      return {
        ...state,
        threadId: action.threadId,
      };
    case CHAT_FETCH_FRESH_MESSAGES_SUCCESS:
      const newHistory = [...state.history];
      action.messages.forEach((message: ChatMessage) => {
        if (!newHistory.find((m) => m.id === message.id)) { //sometimes we get messages we already have
          newHistory.push(message);
        }
      });
      newHistory.sort((a, b) => a.createdOn.diff(b.createdOn));
      return {
        ...state,
        history: newHistory,
      };
    case CHAT_FETCH_FRESH_MESSAGES_FAILURE:
    case CHAT_CREATE_THREAD_FAILURE:
    case CHAT_ADD_MESSAGE_FAILURE:
      return {
        ...state,
        error: action.error,
      };
    default:
      return commonReducer(state, initialState, action);
  }
};

export default chatReducer;
