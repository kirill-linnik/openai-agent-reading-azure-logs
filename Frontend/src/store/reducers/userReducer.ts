import { UserAction, UserState, USER_LOGIN } from "../types/userTypes";
import commonReducer from "./commonReducer";

const initialState: UserState = {
  isLoggedIn: false,
};

const userReducer = (state = initialState, action: UserAction): UserState => {
  switch (action.type) {
    case USER_LOGIN:
      return {
        ...state,
        isLoggedIn: true,
        info: action.info,
      };
    default:
      return commonReducer(state, initialState, action);
  }
};

export default userReducer;
