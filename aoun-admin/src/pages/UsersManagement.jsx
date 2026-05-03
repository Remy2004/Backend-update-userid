import { useState, useEffect } from 'react';
import axios from 'axios';
import { Trash2 } from 'lucide-react';

export default function UsersManagement() {
    const [users, setUsers] = useState([]);

    const fetchUsers = async () => {
        const token = localStorage.getItem('admin_token');
        const res = await axios.get('http://localhost:5055/api/AdminDashboard/users', { headers: { Authorization: `Bearer ${token}` } });
        setUsers(res.data.data);
    };

    useEffect(() => { fetchUsers(); }, []);

    const handleDelete = async (id) => {
        if(window.confirm('هل أنت متأكد من حذف هذا المستخدم؟')) {
            const token = localStorage.getItem('admin_token');
            await axios.delete(`http://localhost:5055/api/AdminDashboard/users/${id}`, { headers: { Authorization: `Bearer ${token}` } });
            fetchUsers();
        }
    };

    return (
        <div className="bg-white p-6 rounded-xl shadow-sm">
            <h3 className="text-xl font-bold mb-4">إدارة المستخدمين</h3>
            <table className="w-full text-right">
                <thead className="bg-gray-100 text-gray-600">
                    <tr><th className="p-3">الاسم</th><th className="p-3">الإيميل</th><th className="p-3">تاريخ التسجيل</th><th className="p-3">إجراءات</th></tr>
                </thead>
                <tbody>
                    {users.map(u => (
                        <tr key={u.id} className="border-b">
                            <td className="p-3 font-semibold">{u.fullName || u.email.split('@')[0]}</td>
                            <td className="p-3 text-gray-500">{u.email}</td>
                            <td className="p-3">-</td>
                            <td className="p-3">
                                <button onClick={() => handleDelete(u.id)} className="text-red-500 hover:text-red-700 bg-red-50 p-2 rounded-lg"><Trash2 size={18} /></button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}