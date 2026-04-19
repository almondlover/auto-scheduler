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
import type { LoginModel } from '@/classes/user';
import { useUserStore } from '@/stores/userStore';
import { storeToRefs } from 'pinia';
import router from '@/router';

const userStore=useUserStore();
const { token } = storeToRefs(userStore);

const login:LoginModel={
    email: '',
    password: ''
}

const handleSubmit = async ()=>{
    await userStore.login(login);
    if (token.value!='') router.push('home');
}
</script>

<template>
    <Card class="m-auto my-15 w-150">
        <CardHeader>
            <CardTitle>Sign in</CardTitle>
            <CardDescription>Log in to your account</CardDescription>
        </CardHeader>
        <CardContent >
            <Form class="flex flex-col gap-5" @submit="handleSubmit">
                <FormField name="name">
                    <FormItem>
                        <FormLabel>Email address</FormLabel>
                        <FormControl>
                            <Input v-model="login.email" required type="text" placeholder="me@mail.com"/>
                        </FormControl>
                    </FormItem>
                </FormField>
                <FormField name="description">
                    <FormItem>
                        <FormLabel>Password</FormLabel>
                        <FormControl>
                            <Input v-model="login.password" required type="password" autocomplete="off" placeholder="Enter a Password"/>
                        </FormControl>
                    </FormItem>
                </FormField>
                <Button type="submit">Log in</Button>
                <RouterLink class="hover:underline" :to="`/register`">Create a new account</RouterLink>
            </Form>
        </CardContent>
    </Card>
</template>