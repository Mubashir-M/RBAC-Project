import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import api from "../api/axios";
import { type Role, type RawRole } from "../types";

interface RoleState {
  roles: Role[];
  loading: boolean;
  error: string | null;
}

const initialState: RoleState = {
  roles: [],
  loading: false,
  error: null,
};

export const fetchRoles = createAsyncThunk("roles/fetchRoles", async () => {
  const response = await api.get("Role/roles");
  return response.data;
});

export const createRole = createAsyncThunk(
  "roles/createRole",
  async (newRole: { name: string; description: string }) => {
    const response = await api.post("Role/roles", newRole);
    return response.data;
  }
);

const roleSlice = createSlice({
  name: "roles",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchRoles.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchRoles.fulfilled, (state, action) => {
        state.loading = false;
        state.roles = action.payload.map((role: RawRole) => ({
          roleId: role.id,
          name: role.name,
          description: role.description,
        }));
      })
      .addCase(fetchRoles.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || "Failed to fetch roles.";
      })

      // createRole handlers
      .addCase(createRole.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(createRole.fulfilled, (state, action) => {
        state.loading = false;
        const role = action.payload as RawRole;
        state.roles.push({
          roleId: role.id,
          name: role.name,
          description: role.description,
        });
      })
      .addCase(createRole.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || "Failed to create role.";
      });
  },
});

export default roleSlice.reducer;
