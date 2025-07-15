import React from "react";
import { useAppDispatch } from "../../features/hooks";
import axios from "../../api/axios";
import { updateUserRoleInRedux } from "../../features/usersListSlice";

interface RoleSelectorProps {
  userId: number;
  currentRole: string;
  username: string;
}

const RoleSelector: React.FC<RoleSelectorProps> = ({
  userId,
  currentRole,
  username,
}) => {
  const dispatch = useAppDispatch();

  const handleChange = async (e: React.ChangeEvent<HTMLSelectElement>) => {
    const selectedRole = e.target.value;

    if (selectedRole === currentRole) return;

    const confirmChange = window.confirm(
      `Change role of ${username} from "${currentRole}" to "${selectedRole}"?`
    );

    if (!confirmChange) return;

    try {
      // Send to backend
      await axios.put(`users/${userId}/assign-role`, {
        roleName: selectedRole,
      });

      // Dispatch Redux update
      dispatch(updateUserRoleInRedux({ userId, newRole: selectedRole }));

      alert("Role updated.");
    } catch (error) {
      console.error(error);
      alert("Failed to update role.");
    }
  };

  return (
    <select value={currentRole} onChange={handleChange}>
      <option value="Admin">Admin</option>
      <option value="User">User</option>
      <option value="Manager">Manager</option>
    </select>
  );
};

export default RoleSelector;
