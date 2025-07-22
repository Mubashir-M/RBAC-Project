import { configureStore } from "@reduxjs/toolkit";
import userReducer from "../features/userSlice";
import userListSlice from "../features/usersListSlice";
import rolesSlice from "../features/roleSlice";
import eventSlice from "../features/EventSlice";

export const store = configureStore({
  reducer: {
    user: userReducer,
    userList: userListSlice,
    roles: rolesSlice,
    events: eventSlice,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
