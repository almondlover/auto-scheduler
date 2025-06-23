import { ref, computed, type Ref } from 'vue'
import { defineStore } from 'pinia'
import type { LoginModel, RegisterModel, User } from '@/classes/user';
import { loginUser, registerUser } from '@/services/userService';

export const useUserStore = defineStore('tiuseresheet', () => {
  const token = ref('');
  const currentUser:Ref<User|null> = ref(null);
  async function register(registerModel:RegisterModel) {
    registerUser(registerModel);
  }
  //should probably set current user outside
  async function login(loginModel:LoginModel) {
    const loggedUser = await loginUser(loginModel);
    if (loggedUser) {
      token.value=loggedUser.token;
      currentUser.value=loggedUser.user;
      localStorage.setItem("userToken", token.value);
    }
  }
  async function logout() {
    token.value = '';
    currentUser.value = null;
    localStorage.removeItem("userToken");
  }
  return { currentUser, token, register, login, logout }
})
