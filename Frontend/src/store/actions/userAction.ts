import { Dispatch } from "redux";
import { ThunkAction } from "redux-thunk";
import { RootState } from "..";
import { UserAction, USER_LOGIN, USER_LOGOUT } from "../types/userTypes";

type UserThunk = ThunkAction<void, RootState, unknown, UserAction>;

function userLogin(info: string): UserAction {
  return {
    type: USER_LOGIN,
    info,
  };
}

function userLogout(): UserAction {
  return {
    type: USER_LOGOUT,
  };
}

export const login2 = (): UserThunk => async (dispatch) => {
  dispatch(userLogin("Logged in via login2"));
};

export function login() {
  return async (dispatch: Dispatch<UserAction>) => {
    dispatch(userLogin("Logged in via login"));
  };
}

export function logout() {
  return async (dispatch: Dispatch<UserAction>) => {
    dispatch(userLogout());
  };
}
