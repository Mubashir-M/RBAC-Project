import { configureStore } from "@reduxjs/toolkit";
import userReducer from "../features/userSlice";
import userListSlice from "../features/usersListSlice";
import rolesSlice from "../features/roleSlice";

export const store = configureStore({
  reducer: {
    user: userReducer,
    userList: userListSlice,
    roles: rolesSlice,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
