import { ref, computed, type Ref } from 'vue'
import { defineStore } from 'pinia'
import type { LoginModel } from '@/classes/user';
import { loginUser, registerUser } from '@/services/userService';

export const useUserStore = defineStore('tiuseresheet', () => {
  const token = ref('');
  async function register(loginModel:LoginModel) {
    registerUser(loginModel);
  }
  //should probably set current user outside
  async function login(loginModel:LoginModel) {
    token.value = await loginUser(loginModel);
    if (token.value!=='') localStorage.setItem("userToken", token.value);
  }
  async function logout() {
    token.value = '';
    localStorage.removeItem("userToken");
  }
  return { token, register, login, logout }
})
