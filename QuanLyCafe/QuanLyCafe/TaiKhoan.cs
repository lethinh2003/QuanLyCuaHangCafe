using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe
{
    internal class TaiKhoan
    {
        string _userName;
        string _password;
        string _firstName;
        string _lastName;
        string _phone;
        string _cccd;
        string _address;
        int _quyenHan;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }
        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }
        public string CCCD
        {
            get { return _cccd; }
            set { _cccd = value; }
        }
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }
        public int QuyenHan
        {
            get { return _quyenHan; }
            set { _quyenHan = value; }
        }
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public TaiKhoan(
            string userName,
            string password,
            string firstName,
            string lastName,
            string phone,
            string cccd,
            string address,
            int quyenHan
        )
        {
            _userName = userName;
            _password = password;
            _firstName = firstName;
            _lastName = lastName;
            _phone = phone;
            _cccd = cccd;
            _address = address;
            _quyenHan = quyenHan;
        }
    }
}
