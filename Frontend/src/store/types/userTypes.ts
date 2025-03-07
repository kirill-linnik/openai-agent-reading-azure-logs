import { CommonState } from "./commonTypes";

export const USER_LOGIN = "@USER/LOGIN";
export const USER_LOGOUT = "@USER/LOGOUT";

export type UserState = CommonState & {
  isLoggedIn: boolean;
};

type UserLoginAction = {
  type: typeof USER_LOGIN;
  info: string;
};

type UserLogoutAction = {
  type: typeof USER_LOGOUT;
};

export type UserAction = UserLoginAction | UserLogoutAction;
