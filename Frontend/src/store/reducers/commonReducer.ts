import {
  CommonState,
  COMMON_REMOVE_ERROR,
  COMMON_REMOVE_INFO,
} from "../types/commonTypes";
import { USER_LOGOUT } from "../types/userTypes";

const commonReducer = (
  state: CommonState,
  initialState: CommonState,
  action: any
): any => {
  switch (action.type) {
    case COMMON_REMOVE_ERROR:
      return {
        ...state,
        error: undefined,
      };
    case COMMON_REMOVE_INFO:
      return {
        ...state,
        info: undefined,
      };
    case USER_LOGOUT:
      return initialState;
    default:
      return state;
  }
};

export default commonReducer;
