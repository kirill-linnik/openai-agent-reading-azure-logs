import { ChatRole } from "../../models/ChatRole";
import {
  CHAT_ADD_MESSAGE_FAILURE,
  CHAT_ADD_MESSAGE_REQUEST,
  CHAT_ADD_MESSAGE_SUCCESS,
  ChatState,
} from "../types/chatTypes";
import commonReducer from "./commonReducer";

const initialState: ChatState = {
  history: [],
};

const chatReducer = (state = initialState, action: any): ChatState => {
  switch (action.type) {
    case CHAT_ADD_MESSAGE_REQUEST:
      return {
        ...state,
        history: [
          ...state.history,
          {
            role: ChatRole.USER,
            content: action.message,
          },
        ],
      };
    case CHAT_ADD_MESSAGE_SUCCESS:
      return {
        ...state,
        history: [...state.history, action.message],
      };
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
