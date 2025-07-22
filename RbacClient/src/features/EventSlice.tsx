import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import api from "../api/axios";
import { AxiosError } from "axios";

import { type Event } from "../types";
interface EventState {
  events: Event[];
  loading: boolean;
  error: string | null;
}

const initialState: EventState = {
  events: [],
  loading: false,
  error: null,
};

export const fetchEvents = createAsyncThunk<
  Event[],
  void,
  { rejectValue: string }
>("events/fetchEvents", async (_, { rejectWithValue }) => {
  try {
    const response = await api.get("Event");
    return response.data;
  } catch (error) {
    const axiosError = error as AxiosError;

    if (axiosError.response && axiosError.response.data) {
      const errorMessage =
        (axiosError.response.data as { message?: string }).message ||
        JSON.stringify(axiosError.response.data);
      return rejectWithValue(errorMessage);
    } else if (axiosError.message) {
      // For network errors or other client-side issues
      return rejectWithValue(axiosError.message);
    }
    return rejectWithValue("An unknown error occurred while fetching events.");
  }
});

const evenSlice = createSlice({
  name: "Events",
  initialState,
  reducers: {},
  extraReducers: (builder) =>
    builder
      .addCase(fetchEvents.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchEvents.fulfilled, (state, action) => {
        state.loading = false;
        state.error = null;
        state.events = action.payload;
      })
      .addCase(fetchEvents.rejected, (state, action) => {
        state.loading = false;
        state.error =
          action.payload || action.error.message || "Failed to load events.";
      }),
});

export default evenSlice.reducer;
