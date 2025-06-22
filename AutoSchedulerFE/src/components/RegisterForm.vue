<script setup lang="ts">
import Card from './ui/card/Card.vue';
import CardHeader from './ui/card/CardHeader.vue';
import CardTitle from './ui/card/CardTitle.vue';
import CardDescription from './ui/card/CardDescription.vue';
import CardContent from './ui/card/CardContent.vue';
import { Form } from 'vee-validate';
import { FormField } from './ui/form';
import FormItem from './ui/form/FormItem.vue';
import FormLabel from './ui/form/FormLabel.vue';
import FormControl from './ui/form/FormControl.vue';
import Input from './ui/input/Input.vue';
import Button from './ui/button/Button.vue';
import type { LoginModel, RegisterModel } from '@/classes/user';
import { useUserStore } from '@/stores/userStore';
import router from '@/router';
import Select from './ui/select/Select.vue';
import SelectTrigger from './ui/select/SelectTrigger.vue';
import SelectValue from './ui/select/SelectValue.vue';
import SelectContent from './ui/select/SelectContent.vue';
import SelectItem from './ui/select/SelectItem.vue';
import { roles } from '@/constants/constants';
import { ref, type Ref } from 'vue';

const userStore=useUserStore();

const register:Ref<RegisterModel>=ref({
    userName:'',
    email: '',
    password: '',
    role: ''
});

const handleSubmit = ()=>{
    userStore.register(register.value);
    router.push('login');
}
</script>

<template>
    <Card class="m-auto my-15 w-150">
        <CardHeader>
            <CardTitle>Sign up</CardTitle>
            <CardDescription>Create a new account</CardDescription>
        </CardHeader>
        <CardContent>
            <Form class="flex flex-col gap-5" @submit="handleSubmit">
                <FormField name="name">
                    <FormItem>
                        <FormLabel>Username</FormLabel>
                        <FormControl>
                            <Input v-model="register.userName" required type="text" placeholder="New user"/>
                        </FormControl>
                    </FormItem>
                </FormField>
                <FormField name="email">
                    <FormItem>
                        <FormLabel>Email address</FormLabel>
                        <FormControl>
                            <Input v-model="register.email" required type="text" placeholder="me@mail.com"/>
                        </FormControl>
                    </FormItem>
                </FormField>
                <FormField name="description">
                    <FormItem>
                        <FormLabel>Password</FormLabel>
                        <FormControl>
                            <Input v-model="register.password" required type="password" placeholder="Enter a Password"/>
                        </FormControl>
                    </FormItem>
                </FormField>
                <FormField name="role">
                    <FormItem>
                        <FormLabel>Account type</FormLabel>
                        <FormControl>
                            <Select v-model="register.role">
                                <SelectTrigger>
                                    <SelectValue placeholder="Choose your account type"/>
                                </SelectTrigger>
                                <SelectContent>
                                    <SelectItem v-for="role in roles" :value="role">
                                        {{ role }}
                                    </SelectItem>
                                </SelectContent>
                            </Select>
                        </FormControl>
                    </FormItem>
                </FormField>
                <Button type="submit">Register</Button>
                <RouterLink :to="`/login`">Sign in</RouterLink>
            </Form>
        </CardContent>
    </Card>
</template>