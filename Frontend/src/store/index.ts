import { configureStore } from "@reduxjs/toolkit";
import { TypedUseSelectorHook, useDispatch, useSelector } from "react-redux";
import { combineReducers } from "redux";
import logger from "redux-logger";
import chatReducer from "./reducers/chatReducer";
import userReducer from "./reducers/userReducer";

const LOCAL_STORAGE_LOCATION = "demo-store";

const rootReducer = combineReducers({
  user: userReducer,
  chat: chatReducer,
});

const loadFromLocalStorage = (): Partial<typeof rootReducer> => {
  try {
    const stateStr = localStorage.getItem(LOCAL_STORAGE_LOCATION);
    return stateStr ? JSON.parse(stateStr) : {};
  } catch (e) {
    console.error(e);
    return {};
  }
};

const store = configureStore({
  reducer: rootReducer,
  middleware: (getDefaultMiddleware) => {
    let middleware = getDefaultMiddleware({
      serializableCheck: false, //as we are using Map, it seems to be not serializable, so we have to remove this check here
    });

    if (process.env.NODE_ENV !== "production") {
      return middleware.concat(logger);
    }

    return middleware;
  },
  preloadedState: loadFromLocalStorage(),
});

const stateObjectsToPersist: Array<keyof RootState> = ["user"];

const saveToLocalStorage = (state: RootState) => {
  try {
    let stateToPersist: any = {};
    let stateProperty: keyof RootState;
    for (stateProperty in state) {
      if (!stateObjectsToPersist.includes(stateProperty)) {
        continue;
      }
      stateToPersist[stateProperty] = state[stateProperty];
    }
    localStorage.setItem(
      LOCAL_STORAGE_LOCATION,
      JSON.stringify(stateToPersist)
    );
  } catch (e) {
    console.log(e);
  }
};

store.subscribe(() => {
  saveToLocalStorage(store.getState());
});

export type RootState = ReturnType<typeof rootReducer>;
export type AppDispatch = typeof store.dispatch;
export const useAppDispatch = () => useDispatch<AppDispatch>();
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;

export default store;
